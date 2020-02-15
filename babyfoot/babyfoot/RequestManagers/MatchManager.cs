using babyfoot.Models;
using babyfoot.Rules;
using babyfoot.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.RequestManagers
{
    public class MatchManager
    {
        private BabyfootDbContext context;

        public MatchManager(BabyfootDbContext context)
        {
            this.context = context;
        }



        public void RemoveGoal(String pseudo, Match match)
        {
            var player_goal = match.GoalsOfMatch.Where(t => t.Player.Pseudo.Equals(pseudo)).FirstOrDefault();

            if (player_goal == null)
                throw new Exception();

            if (player_goal.Goals == 0)
                throw new Exception();

            --player_goal.Goals;

            context.Entry(match).State = EntityState.Modified;

            context.SaveChanges();
        }

        public void AddGoal(String pseudo, Match match)
        {
            var player_goal = match.GoalsOfMatch.Where(t => t.Player.Pseudo.Equals(pseudo)).FirstOrDefault();

            if (player_goal == null)
                throw new Exception();

            if (match.Stage == MatchStage.Pool && player_goal.Goals == BabyfootRules.PoolMaxGoals)
                throw new Exception();

            ++player_goal.Goals;

            context.Entry(player_goal).State = EntityState.Modified;

            context.SaveChanges();
        }

        public Match Create(MatchMakingView view, String tournament_token, MatchStage stage, int order)
        {
            var date = DateTime.UtcNow;
            var time = BitConverter.GetBytes(date.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            String token = Convert.ToBase64String(time.Concat(key).ToArray());

            var match = new Match
            {
                Token = token,
                Order = order,
                Stage = stage,
                State = MatchState.NotStarted,
                ElapsedSeconds = 0,
                StartDate = null
            };

            match.Tournament = context.Tournaments.First(t => t.Token.Equals(tournament_token));
            match.TeamsOfMatch = new List<MatchTeam>();
            match.GoalsOfMatch = new List<PlayerGoal>();

            // save (create match id and set tournament id)
            context.Add(match);
            context.SaveChanges();


            // get pseudos of view
            var pseudos = new List<String>(view.SelectMany(t => t));

            // get players and teams views
            var (vt1, vt2) = (view[0], view[1]);
            var (vp1, vp2) = (vt1[0], vt1[1]);
            var (vp3, vp4) = (vt2[0], vt2[1]);

            // get team-players
            var team_players_query = context.TeamPlayers
                .Include(t => t.Team)
                .Include(t => t.Player)
                .Where(t => pseudos.Contains(t.Player.Pseudo));

            var team_players = new List<PlayerTeam>(team_players_query);


            // get players in bijection with the view
            var (tp1, tp2) = (team_players.First(t => t.Player.Pseudo.Equals(vp1)), team_players.First(t => t.Player.Pseudo.Equals(vp2)));
            var (tp3, tp4) = (team_players.First(t => t.Player.Pseudo.Equals(vp3)), team_players.First(t => t.Player.Pseudo.Equals(vp4)));

            var (t1, t2) = (tp1.Team, tp3.Team);
            var (p1, p2) = (tp1.Player, tp2.Player);
            var (p3, p4) = (tp3.Player, tp4.Player);

            // set team points
            //t1.Points = vt1.Points;
            //t2.Points = vt2.Points;

            // set goals of match
            match.GoalsOfMatch = new List<PlayerGoal>
            {
                new PlayerGoal { Match = match, Player = p1, Goals = 0 },
                new PlayerGoal { Match = match, Player = p2, Goals = 0 },
                new PlayerGoal { Match = match, Player = p3, Goals = 0 },
                new PlayerGoal { Match = match, Player = p4, Goals = 0 }
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

        public void End(Match match)
        {
            if (match.State == MatchState.NotStarted || match.State == MatchState.Ended)
                throw new Exception();

            match.State = MatchState.Ended;
            match.ElapsedSeconds += (int)(DateTime.UtcNow - match.RestartDate).Value.TotalSeconds;

            var teams = match.TeamsOfMatch.Select(t => t.Team).DistinctBy(t => t.TeamId).ToList();
            var players = teams.SelectMany(t => t.PlayersOfTeam).Select(t => t.Player).ToList();
            var goals = new List<int>
            {
                match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(players[0].Pseudo)).Goals,
                match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(players[1].Pseudo)).Goals,
                match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(players[2].Pseudo)).Goals,
                match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(players[3].Pseudo)).Goals
            };

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

            context.Entry(match).State = EntityState.Modified;

            context.SaveChanges();
        }

        public void Start(Match match)
        {
            if (match.State != MatchState.NotStarted)
                throw new Exception();

            match.State = MatchState.InProgress;
            match.StartDate = DateTime.UtcNow;
            match.RestartDate = DateTime.UtcNow;

            context.SaveChanges();
        }

        public void Pause(Match match)
        {
            if (match.State != MatchState.InProgress)
                throw new Exception();

            match.State = MatchState.Pause;
            match.ElapsedSeconds += (int)(DateTime.UtcNow - match.RestartDate).Value.TotalSeconds;

            context.SaveChanges();
        }

        public void Restart(Match match)
        {
            if (match.State != MatchState.Pause)
                throw new Exception();

            match.State = MatchState.InProgress;
            match.RestartDate = DateTime.UtcNow;

            context.SaveChanges();
        }


        /*
        public void Score(MatchScoresView view, Match match)
        {

            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(view[0].Players[0].Pseudo)).Goals += view[0].Players[0].Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(view[0].Players[1].Pseudo)).Goals += view[0].Players[1].Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(view[1].Players[0].Pseudo)).Goals += view[1].Players[0].Goals;
            match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(view[1].Players[1].Pseudo)).Goals += view[1].Players[1].Goals;

            match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(view[0].Players[0].Pseudo))).Team.Points = view[0].Points;
            match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(view[1].Players[0].Pseudo))).Team.Points = view[1].Points;

            match.GoalsOfMatch.Select(t => t);

            match.State = MatchState.Ended;

            context.Entry(match).State = EntityState.Modified;

            context.SaveChanges();
        }
        */
    }

}
