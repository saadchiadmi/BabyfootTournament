using babyfoot.Models;
using babyfoot.Rules;
using babyfoot.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.RequestManagers
{
    public class Synchronizer
    {
        private BabyfootDbContext context;
        private TournamentManager tournament_manager;
        private MatchManager match_manager;

        public Synchronizer(BabyfootDbContext context)
        {
            this.context = context;
            this.tournament_manager = new TournamentManager(context);
            this.match_manager = new MatchManager(context);
        }



        public List<TournamentTokenRenameView> Synchronize(ClientStateView view)
        {
            // synchronize players
            // (add new client players)
            var known_pseudos = context.Players.Select(t => t.Pseudo).ToList();
            var unknown_pseudos = view.Pseudos.Where(p => !known_pseudos.Contains(p)).ToList();

            PlayerManager player_manager = new PlayerManager(context);

            player_manager.AddMany(unknown_pseudos);


            // get tournaments
            var tournaments = context.Tournaments
                .Include(t => t.Matches)
                    .ThenInclude(t => t.TeamsOfMatch)
                        .ThenInclude(t => t.Team)
                            .ThenInclude(t => t.PlayersOfTeam)
                                .ThenInclude(t => t.Player)
                .Include(t => t.Matches)
                    .ThenInclude(t => t.GoalsOfMatch)
                    .ToList();

            var token_renames = new List<TournamentTokenRenameView>();

            // synchronize tournaments
            var new_ended_tournaments = new List<Tournament>();
            foreach (var vtournament in view.Tournaments)
            {
                var tournament = tournaments.FirstOrDefault(t => t.Token.Equals(vtournament.Token));

                // new client tournament
                if (tournament == null)
                {
                    // client tournament should not be scored
                    if (vtournament.State == TournamentState.Scored)
                        throw new Exception();

                    // add tournament
                    var new_tournament = Create(vtournament);

                    token_renames.Add(new TournamentTokenRenameView { Initial = vtournament.Token, Token = new_tournament.Token });

                    if (new_tournament.State == TournamentState.Ended)
                        new_ended_tournaments.Add(new_tournament);
                }
                // client tournament already exist as server tournament
                else
                {
                    var need_score = Synchronize(vtournament, tournament);
                    if (need_score)
                        new_ended_tournaments.Add(tournament);
                }

            }

            // score tournaments

            // get the date of the last scored tournament (if exist)
            var scored_tournament_dates = context.Tournaments.Where(t => t.State == TournamentState.Scored).Select(t => t.CreateDate).ToList();
            var last_date = new DateTime();
            var last_date_exist = false;
            if (scored_tournament_dates.Count() > 0)
            {
                last_date = scored_tournament_dates.Max();
                last_date_exist = true;
            }

            // select new ended client tournaments that has been created after last scored tournament (if exist)
            List<Tournament> to_score_ordered_tournaments = null;
            if (last_date_exist)
            {
                to_score_ordered_tournaments = new_ended_tournaments.Where(t => last_date.CompareTo(t.CreateDate) < 0).ToList();
                to_score_ordered_tournaments.Sort();
            }
            else
            {
                to_score_ordered_tournaments = new_ended_tournaments;
                to_score_ordered_tournaments.Sort();
            }

            // score new ended client tournaments
            // (creation date order)
            foreach (var tournament in to_score_ordered_tournaments)
            {
                tournament_manager.Score(tournament);
            }

            return token_renames;
        }



        private bool Synchronize(TournamentView vtournament, Tournament tournament)
        {
            // server tournament is in pool
            if (tournament.State == TournamentState.Pool)
            {
                // client tournament is in pool too
                if (vtournament.State == TournamentState.Pool)
                {
                    // override server match by client match only if client match is more "advanced" (more goals or advanced state)
                    foreach (var vmatch in vtournament.Matches)
                    {
                        var match = tournament.Matches.FirstOrDefault(t => t.Token.Equals(vmatch.Token));

                        if (match == null)
                            throw new Exception();

                        Override(vmatch, match);
                    }
                    return false;
                }
                // client tournament is more advanced
                else if (vtournament.State == TournamentState.Semifinal)
                {
                    Override(vtournament, tournament);
                    return false;
                }
                else if (vtournament.State == TournamentState.Final)
                {
                    Override(vtournament, tournament);
                    return false;
                }
                else if (vtournament.State == TournamentState.Ended)
                {
                    Override(vtournament, tournament);
                    return true;
                }
                else
                {
                    throw new Exception();
                }
            }
            // server tournament is in semifinal
            else if (tournament.State == TournamentState.Semifinal)
            {
                // client tournament is in pool
                if (vtournament.State == TournamentState.Pool)
                {
                    // ignore client tournament
                    return false;
                }
                // client tournament is in semifinal too
                else if (vtournament.State == TournamentState.Semifinal)
                {
                    // ignore client match if not the same as server
                    // or override server match if the same and more advanced

                    var vsemifinals = vtournament.Matches.Where(t => t.Stage == MatchStage.Semifinal).ToList();

                    if (vsemifinals.Count() != 2)
                        throw new Exception();

                    var semifinals = new List<Match>
                    {
                        tournament.Matches.FirstOrDefault(t => t.Token.Equals(vsemifinals[0].Token)),
                        tournament.Matches.FirstOrDefault(t => t.Token.Equals(vsemifinals[1].Token))
                    };

                    if (semifinals[0] == null || semifinals[1] == null)
                        throw new Exception();

                    Override(vsemifinals[0], semifinals[0]);
                    Override(vsemifinals[1], semifinals[1]);

                    return false;
                }
                // client tournament is in final
                // client tournament is more advanced
                else if(vtournament.State == TournamentState.Final)
                {
                    Override(vtournament, tournament);
                    return false;
                }
                // client tournament is ended
                // client tournament is more advanced
                else if (vtournament.State == TournamentState.Ended)
                {
                    Override(vtournament, tournament);
                    return true;
                }
                // client tournament is scored
                // impossible
                else
                {
                    throw new Exception();
                }
            }
            // server tournament is in final
            else if(tournament.State == TournamentState.Final)
            {
                // client tournament is in pool or in semifinal
                if (vtournament.State == TournamentState.Pool || vtournament.State == TournamentState.Semifinal)
                {
                    // ignore client tournament
                    return false;
                }
                // client tournament is in final too
                else if(vtournament.State == TournamentState.Final)
                {
                    var match = tournament.Matches.FirstOrDefault(t => t.Stage == MatchStage.Final);

                    if (match == null)
                        throw new Exception();

                    var vmatch = vtournament.Matches.FirstOrDefault(t => t.Stage == MatchStage.Final);

                    if (vmatch == null)
                        throw new Exception();

                    Override(vmatch, match);

                    return false;
                }
                // client tournament is ended
                // client tournament is more advanced
                else if (vtournament.State == TournamentState.Ended)
                {
                    Override(vtournament, tournament);
                    return true;
                }
                // client tournament is scored
                // impossible
                else
                {
                    throw new Exception();
                }

            }
            // server tournament is ended
            else if(tournament.State == TournamentState.Ended)
            {
                if (vtournament.State == TournamentState.Scored)
                    throw new Exception();
                // client tournament is ignored
                return false;
            }
            // server tournament is scored
            else
            {
                // client tournament is ignored
                return false;
            }
        }



        private Tournament Override(TournamentView view, Tournament tournament)
        {
            // override state
            tournament.State = view.State;

            // override points
            var teams_get = tournament.Matches.SelectMany(t => t.TeamsOfMatch).Select(t => t.Team).DistinctBy(t => t.TeamId);
            var teams = new List<Team>(teams_get);

            foreach (TournamentTeamView mview in view.Teams)
            {
                List<String> vpseudos = mview.Pseudos;
                var team = teams.First(t => t.PlayersOfTeam.Any(t => vpseudos.Any(p => p.Equals(t.Player.Pseudo))));
                team.Points = mview.Points;
                context.Entry(team).State = EntityState.Modified;
            }
            context.SaveChanges();

            foreach (MatchView mview in view.Matches)
            {
                var match = tournament.Matches.FirstOrDefault(t => t.Token.Equals(mview.Token));
                if (match == null)
                    Create(mview, tournament);
                else
                    Override(mview, match);
            }

            context.Entry(tournament).State = EntityState.Modified;
            context.SaveChanges();

            return tournament;
        }



        private void Override(MatchView vmatch, Match match)
        {
            if (match.Stage != vmatch.Stage)
                throw new Exception();

            bool client_match_is_advanced
                = match.State != MatchState.Ended && 
                (vmatch.State == MatchState.Ended
                || match.State == MatchState.NotStarted && vmatch.State != MatchState.NotStarted
                || match.State != MatchState.NotStarted && vmatch.State != MatchState.NotStarted && match.GoalsOfMatch.Select(t => t.Goals).Aggregate((a, b) => a + b) < vmatch.Teams.SelectMany(t => t.Select(t => t.Goals)).Aggregate((a, b) => a + b));

            if (client_match_is_advanced)
            {
                match.Order = vmatch.Order;
                match.Stage = vmatch.Stage;
                match.State = vmatch.State;
                match.ElapsedSeconds = vmatch.ElapsedSeconds;

                match.StartDate = vmatch.StartDate;
                match.RestartDate = vmatch.RestartDate;

                var player_goals = new List<PlayerGoal>
                {
                    match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(vmatch.Teams[0][0].Pseudo)),
                    match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(vmatch.Teams[0][1].Pseudo)),
                    match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(vmatch.Teams[1][0].Pseudo)),
                    match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(vmatch.Teams[1][1].Pseudo)),
                };

                // set goals
                player_goals[0].Goals = vmatch.Teams[0][0].Goals;
                player_goals[1].Goals = vmatch.Teams[0][1].Goals;
                player_goals[2].Goals = vmatch.Teams[1][0].Goals;
                player_goals[3].Goals = vmatch.Teams[1][1].Goals;

                // set points
                if (vmatch.State == MatchState.Ended)
                {
                    var teams = new List<Team>
                {
                    match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(player_goals[0].Player.Pseudo))).Team,
                    match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(player_goals[2].Player.Pseudo))).Team
                };

                    var goals = player_goals.Select(t => t.Goals).ToList();

                    var team_goals = new List<int>
                {
                    goals[0] + goals[1],
                    goals[2] + goals[3]
                };

                    if (team_goals[0] < team_goals[1])
                    {
                        teams[0].Points += BabyfootRules.LossPoints;
                        teams[1].Points += BabyfootRules.WinPoints;
                    }
                    else if (team_goals[0] > team_goals[1])
                    {
                        teams[0].Points += BabyfootRules.WinPoints;
                        teams[1].Points += BabyfootRules.LossPoints;
                    }
                    else
                    {
                        teams[0].Points += BabyfootRules.DrawPoints;
                        teams[1].Points += BabyfootRules.DrawPoints;
                    }
                }

                context.Entry(match).State = EntityState.Modified;

                context.SaveChanges();
            }
        }



        public Tournament Create(TournamentView view)
        {
            // assume players in the view are known

            // replace client token by a server token
            var date = view.CreateDate;
            var time = BitConverter.GetBytes(date.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            String tournament_token = Convert.ToBase64String(time.Concat(key).ToArray());

            // add tournament id
            var tournament = new Tournament
            {
                Token = tournament_token,
                State = view.State,
                CreateDate = view.CreateDate,
                Matches = new List<Match>()
            };

            // save (create tournament id)
            context.Add(tournament);
            context.SaveChanges();

            var vpseudos = view.Teams.SelectMany(t => t.Pseudos);
            var players = context.Players.Where(t => vpseudos.Contains(t.Pseudo));

            // set teams
            var teams = new List<Team>();
            foreach (var vteam in view.Teams)
            {
                var team = new Team { Points = vteam.Points };
                context.Add(team);
                teams.Add(team);
            }

            // save (create team ids)
            context.SaveChanges();

            // set team-players
            int i = 0;
            foreach (var vteam in view.Teams)
            {
                var team = teams[i];
                foreach (var vpseudo in vteam.Pseudos)
                {
                    var player = players.First(t => t.Pseudo.Equals(vpseudo));
                    var pt = new PlayerTeam { Player = player, Team = team };
                    context.Add(pt);
                }

                ++i;
            }
            // save
            context.SaveChanges();

            // set matches
            tournament.Matches = new List<Match>(view.Matches.Select(t => Create(t, tournament)));

            // save
            context.Entry(tournament).State = EntityState.Modified;
            context.SaveChanges();

            return tournament;
        }



        public Match Create(MatchView view, Tournament tournament)
        {
            var match = new Match
            {
                Token = view.Token,
                Order = view.Order,
                Stage = view.Stage,
                State = view.State,
                ElapsedSeconds = view.ElapsedSeconds,
                StartDate = view.StartDate
            };

            match.Tournament = tournament;
            match.TeamsOfMatch = new List<MatchTeam>();
            match.GoalsOfMatch = new List<PlayerGoal>();

            // save (create match id and set tournament id)
            context.Add(match);
            context.SaveChanges();


            // get pseudos of view
            var pseudos = new List<String>(view.Teams.SelectMany(t => t).Select(t => t.Pseudo));

            // get players and teams views
            var (vt1, vt2) = (view.Teams[0], view.Teams[1]);
            var (vp1, vp2) = (vt1[0], vt1[1]);
            var (vp3, vp4) = (vt2[0], vt2[1]);

            // get team-players
            var team_players_query = context.TeamPlayers
                .Include(t => t.Team)
                .Include(t => t.Player)
                .Where(t => pseudos.Contains(t.Player.Pseudo));

            var team_players = new List<PlayerTeam>(team_players_query);


            // get players in bijection with the view
            var (tp1, tp2) = (team_players.First(t => t.Player.Pseudo.Equals(vp1.Pseudo)), team_players.First(t => t.Player.Pseudo.Equals(vp2.Pseudo)));
            var (tp3, tp4) = (team_players.First(t => t.Player.Pseudo.Equals(vp3.Pseudo)), team_players.First(t => t.Player.Pseudo.Equals(vp4.Pseudo)));

            var (t1, t2) = (tp1.Team, tp3.Team);
            var (p1, p2) = (tp1.Player, tp2.Player);
            var (p3, p4) = (tp3.Player, tp4.Player);

            // set team points
            //t1.Points = vt1.Points;
            //t2.Points = vt2.Points;

            // set goals of match
            match.GoalsOfMatch = new List<PlayerGoal>
            {
                new PlayerGoal { Match = match, Player = p1, Goals = vp1.Goals },
                new PlayerGoal { Match = match, Player = p2, Goals = vp2.Goals },
                new PlayerGoal { Match = match, Player = p3, Goals = vp3.Goals },
                new PlayerGoal { Match = match, Player = p4, Goals = vp4.Goals }
            };

            context.Entry(match).State = EntityState.Modified;
            context.SaveChanges();

            // set teams of match
            match.TeamsOfMatch = new List<MatchTeam>
            {
                new MatchTeam { Match = match, Team = t1 },
                new MatchTeam { Match = match, Team = t2 }
            };

            // save
            context.Entry(match).State = EntityState.Modified;
            context.SaveChanges();

            return match;
        }


    }
}
