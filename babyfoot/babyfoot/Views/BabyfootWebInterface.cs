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









        public PlayerGoalView View(PlayerGoal entity)
        {
            return new PlayerGoalView()
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
                Points = entity.Team.Points,
                Players = new List<PlayerGoalView>()
                {
                    View(entity.Match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p1.Pseudo))),
                    View(entity.Match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p2.Pseudo)))
                }
            };
            return view;
        }

        public TeamView View(Team entity)
        {
            var p1 = entity.PlayersOfTeam.ElementAt(0).Player;
            var p2 = entity.PlayersOfTeam.ElementAt(1).Player;

            var view = new TeamView()
            {
                Points = entity.Points,
                Players = new List<PlayerView>()
                {
                    View(p1),
                    View(p2)
                }
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

        public Match Entity(MatchView view)
        {
            var match = EntityToSaveAsync(view).Result;
            context.SaveChanges();
            return match;
        }

        public async Task<Match> EntityToSaveAsync(MatchView view)
        {
            var match = await context.Matches
                .Where(t => t.Token.Equals(view.Token))
                    .Include(t => t.GoalsOfMatch)
                    .Include(t => t.TeamsOfMatch)
                        .ThenInclude(t => t.Team)
                        .ThenInclude(t => t.PlayersOfTeam)
                        .ThenInclude(t => t.Player)
                            .FirstOrDefaultAsync();

            match.Order = view.Order;
            match.Stage = view.Stage;
            match.State = view.State;
            match.ElapsedSeconds = view.ElapsedSeconds;

            match.StartDate = view.StartDate;

            var (t1, t2) = (view.Teams[0], view.Teams[1]);
            var (p1, p2) = (t1.Players[0], t1.Players[1]);
            var (p3, p4) = (t2.Players[0], t2.Players[1]);

            match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(p1.Pseudo))).Team.Points = t1.Points;
            match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(p3.Pseudo))).Team.Points = t2.Points;

            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p1.Pseudo)).Goals = p1.Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p2.Pseudo)).Goals = p2.Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p3.Pseudo)).Goals = p3.Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p4.Pseudo)).Goals = p4.Goals;

            context.Entry(match).State = EntityState.Modified;

            return match;
        }


        public Match NewEntity(MatchView view, String tournament_token)
        {
            var match = NewEntityAsync(view, tournament_token).Result;
            return match;
        }

        public async Task<Match> NewEntityAsync(MatchView view, String tournament_token)
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
            var pseudos = new List<String>(view.Teams.SelectMany(t => t.Players).Select(t => t.Pseudo));

            // get players and teams views
            var (vt1, vt2) = (view.Teams[0], view.Teams[1]);
            var (vp1, vp2) = (vt1.Players[0], vt1.Players[1]);
            var (vp3, vp4) = (vt2.Players[0], vt2.Players[1]);

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
            t1.Points = vt1.Points;
            t2.Points = vt2.Points;

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











        public TournamentView View(Tournament entity)
        {
            var view = new TournamentView()
            {
                Token = entity.Token,
                CreateDate = entity.CreateDate,
                State = entity.State,
                Teams = new List<TeamView>(entity.Matches.SelectMany(t => t.TeamsOfMatch).DistinctBy(t => t.TeamId).Select(t => View(t.Team))),
                Matches = new List<MatchView>(entity.Matches.Select(t => View(t)))
            };
            return view;
        }

        public Tournament Entity(TournamentView view)
        {
            var tournament = EntityAsync(view).Result;
            return tournament;
        }

        public async Task<Tournament> EntityAsync(TournamentView view)
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

            List<Match> matches = new List<Match>();
            foreach (MatchView mview in view.Matches)
            {
                var match = tournament.Matches.FirstOrDefault(t => t.Token.Equals(mview.Token));
                if (match == null)
                    match = NewEntity(mview, tournament.Token);
                else
                    match = Entity(mview);
            }

            context.Entry(tournament).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return tournament;
        }



        public async Task<Tournament> NewEntityAsync(TournamentView view)
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
            await context.SaveChangesAsync();

            var vpseudos = view.Teams.SelectMany(t => t.Players.Select(t => t.Pseudo));
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
            await context.SaveChangesAsync();

            // set team-players and set players
            int i = 0;
            foreach (var vteam in view.Teams)
            {
                var team = teams[i];
                foreach(var vplayer in vteam.Players)
                {
                    var player = players.First(t => t.Pseudo.Equals(vplayer.Pseudo));
                    player.Score = vplayer.Score;
                    player.Goals = vplayer.Goals;
                    player.Champions = vplayer.Champions;
                    var pt = new PlayerTeam { Player = player, Team = team };
                    context.Add(pt);
                }

                ++i;
            }
            // save
            await context.SaveChangesAsync();

            // set matches
            tournament.Matches = new List<Match>(view.Matches.Select(t => NewEntity(t, tournament.Token)));

            // save
            context.Entry(tournament).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return tournament;
        }


    }
}
