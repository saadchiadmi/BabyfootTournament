using babyfoot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class BabyfootWebInterface
    {
        private readonly BabyfootDbContext context;

        public BabyfootWebInterface(BabyfootDbContext context)
        {
            this.context = context;
        }

        // View = deduce view of an entity
        // Entity = extract entity from db according to the view
        // NewEntity = creates an non existing entity, that may be added to the db after the call




        public PlayerView View(Player entity)
        {
            return new PlayerView()
            {
                Pseudo = entity.Pseudo,
                Score = entity.Score,
                Goals = entity.Goals,
                Champions = entity.Champions
            };
        }









        public MatchPlayerView View(PlayerGoal entity)
        {
            return new MatchPlayerView()
            {
                Pseudo = entity.Player.Pseudo,
                Goals = entity.Goals
            };
        }

        public MatchTeamView View(MatchTeam entity)
        {
            var p1 = entity.Team.PlayersOfTeam.ElementAt(0).Player;
            var p2 = entity.Team.PlayersOfTeam.ElementAt(1).Player;

            var view = new MatchTeamView()
            {
                View(entity.Match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p1.Pseudo))),
                View(entity.Match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p2.Pseudo)))
            };
            return view;
        }

        public TournamentTeamView View(Team entity)
        {
            var p1 = entity.PlayersOfTeam.ElementAt(0).Player;
            var p2 = entity.PlayersOfTeam.ElementAt(1).Player;

            var view = new TournamentTeamView()
            {
                Points = entity.Points,
                Pseudos = new List<String> { p1.Pseudo, p2.Pseudo }
            };
            return view;
        }











        public MatchView View(Match match)
        {
            var view = new MatchView()
            {
                Token = match.Token,
                Order = match.Order,
                StageStr = match.Stage.ToString(),
                StateStr = match.State.ToString(),
                StartDate = match.StartDate,
                ElapsedSeconds = match.ElapsedSeconds
            };

            view.Teams = new List<MatchTeamView>
            {
                View(match.TeamsOfMatch.ElementAt(0)),
                View(match.TeamsOfMatch.ElementAt(1))
            };
            return view;
        }

        public async Task<Match> ModifyAsync(MatchView view, Match match)
        {
            ModifyToSave(view, match);
            await context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> ModifyAsync(MatchView view)
        {
            var match = await ModifyToSaveAsync(view);
            await context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> ModifyToSaveAsync(MatchView view)
        {
            var match = await context.Matches
                .Where(t => t.Token.Equals(view.Token))
                    .Include(t => t.GoalsOfMatch)
                    .Include(t => t.TeamsOfMatch)
                        .ThenInclude(t => t.Team)
                        .ThenInclude(t => t.PlayersOfTeam)
                        .ThenInclude(t => t.Player)
                            .FirstOrDefaultAsync();

            ModifyToSave(view, match);

            return match;
        }

        public void ModifyToSave(MatchView view, Match match)
        {

            match.Order = view.Order;
            match.Stage = view.Stage;
            match.State = view.State;
            match.ElapsedSeconds = view.ElapsedSeconds;

            match.StartDate = view.StartDate;

            var (t1, t2) = (view.Teams[0], view.Teams[1]);
            var (p1, p2) = (t1[0], t1[1]);
            var (p3, p4) = (t2[0], t2[1]);

            //match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(p1.Pseudo))).Team.Points = t1.Points;
            //match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(p3.Pseudo))).Team.Points = t2.Points;

            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p1.Pseudo)).Goals = p1.Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p2.Pseudo)).Goals = p2.Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p3.Pseudo)).Goals = p3.Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p4.Pseudo)).Goals = p4.Goals;

            context.Entry(match).State = EntityState.Modified;
        }

        public async Task<Match> CreateAsync(MatchView view, String tournament_token)
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

            match.Tournament = context.Tournaments.First(t => t.Token.Equals(tournament_token));
            match.TeamsOfMatch = new List<MatchTeam>();
            match.GoalsOfMatch = new List<PlayerGoal>();

            // save (create match id and set tournament id)
            context.Add(match);
            await context.SaveChangesAsync();


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

            // set teams of match
            match.TeamsOfMatch = new List<MatchTeam>
            {
                new MatchTeam { Match = match, Team = t1 },
                new MatchTeam { Match = match, Team = t2 }
            };

            // save
            context.Entry(match).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return match;
        }

        public Match Create(MatchView view, String tournament_token)
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

            match.Tournament = context.Tournaments.First(t => t.Token.Equals(tournament_token));
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

        public bool CheckState(MatchView view, Match entity)
        {
            if (!(entity.Stage.Equals(view.Stage)))
                return false;
            if (!(entity.State <= view.State))
                return false;
            if (!(entity.StartDate.Equals(view.StartDate)))
                return false;
            if (!(entity.ElapsedSeconds <= view.ElapsedSeconds))
                return false;
            return true;
        }









        public TournamentView View(Tournament entity)
        {
            var view = new TournamentView()
            {
                Token = entity.Token,
                CreateDate = entity.CreateDate,
                StateStr = entity.State.ToString(),
                Teams = new List<TournamentTeamView>(entity.Matches.SelectMany(t => t.TeamsOfMatch).DistinctBy(t => t.TeamId).Select(t => View(t.Team))),
                Matches = new List<MatchView>(entity.Matches.Select(t => View(t)))
            };
            return view;
        }

        public async Task<Tournament> ModifyAsync(TournamentView view)
        {
            var tournament = await context.Tournaments
                .Where(t => t.Token.Equals(view.Token))
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                            .FirstAsync();

            tournament.State = view.State;

            var teams_get = tournament.Matches.SelectMany(t => t.TeamsOfMatch).Select(t => t.Team).DistinctBy(t => t.TeamId);
            var teams = new List<Team>(teams_get);

            foreach (TournamentTeamView mview in view.Teams)
            {
                List<String> vpseudos = mview.Pseudos;
                var team = teams.First(t => t.PlayersOfTeam.Any(t => vpseudos.Any(p => p.Equals(t.Player.Pseudo))));
                team.Points = mview.Points;
                context.Entry(team).State = EntityState.Modified;
            }
            await context.SaveChangesAsync();

            foreach (MatchView mview in view.Matches)
            {
                var match = tournament.Matches.FirstOrDefault(t => t.Token.Equals(mview.Token));
                if (match == null)
                    match = CreateAsync(mview, tournament.Token).Result;
                else
                    match = ModifyAsync(mview).Result;
            }

            context.Entry(tournament).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return tournament;
        }



        public Tournament Create(TournamentView view)
        {
            // assume players in the view are known

            // add tournament id
            var tournament = new Tournament
            {
                Token = view.Token,
                State = view.State,
                CreateDate = view.CreateDate,
                Matches = new List<Match> { }
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
                foreach(var vpseudo in vteam.Pseudos)
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
            tournament.Matches = new List<Match>(view.Matches.Select(t => CreateAsync(t, tournament.Token).Result));

            // save
            context.Entry(tournament).State = EntityState.Modified;
            context.SaveChanges();

            return tournament;
        }

        public async Task<Tournament> CreateAsync(TournamentView view)
        {
            // assume players in the view are known

            // add tournament id
            var tournament = new Tournament
            {
                Token = view.Token,
                State = view.State,
                CreateDate = view.CreateDate,
                Matches = new List<Match> { }
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
            tournament.Matches = new List<Match>(view.Matches.Select(t => Create(t, tournament.Token)));

            // save
            context.Entry(tournament).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return tournament;
        }

    }
}
