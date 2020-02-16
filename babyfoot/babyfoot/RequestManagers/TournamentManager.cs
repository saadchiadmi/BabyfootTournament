using babyfoot.Elo;
using babyfoot.Models;
using babyfoot.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static babyfoot.Elo.PlayerPerformanceCalculator;

namespace babyfoot.RequestManagers
{
    public class TournamentManager
    {

        private BabyfootDbContext context;
        private MatchManager match_manager;

        public TournamentManager(BabyfootDbContext context)
        {
            this.context = context;
            this.match_manager = new MatchManager(context);
        }




        public Match AddFinal(MatchMakingView vfinal, Tournament tournament)
        {
            var final = match_manager.Create(vfinal, tournament.Token, MatchStage.Final, 0);

            tournament.State = TournamentState.Final;

            context.SaveChanges();

            return final;
        }

        public List<Match> AddSemifinals(List<MatchMakingView> vsemifinals, Tournament tournament)
        {
            var semifinals = new List<Match>
            {
                match_manager.Create(vsemifinals[0], tournament.Token, MatchStage.Semifinal, 1),
                match_manager.Create(vsemifinals[1], tournament.Token, MatchStage.Semifinal, 2)
            };

            tournament.State = TournamentState.Semifinal;

            context.SaveChanges();

            return semifinals;
        }

        public Tournament Create(List<MatchMakingView> view)
        {
            // assume players in the view are known

            var date = DateTime.UtcNow;
            var time = BitConverter.GetBytes(date.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            String tournament_token = Convert.ToBase64String(time.Concat(key).ToArray());

            // add tournament id
            var tournament = new Tournament
            {
                Token = tournament_token,
                State = TournamentState.Pool,
                CreateDate = date,
                Matches = new List<Match>()
            };

            // save (create tournament id)
            context.Add(tournament);
            context.SaveChanges();

            var vpseudos = view.SelectMany(t => t.SelectMany(t => t)).Distinct();
            var players = context.Players.Where(t => vpseudos.Contains(t.Pseudo));

            // set teams
            var teams = new List<Team>();
            foreach (var vteam in view)
            {
                var team = new Team { Points = 0 };
                context.Add(team);
                teams.Add(team);
            }

            // save (create team ids)
            context.SaveChanges();

            // set team-players
            int i = 0;
            foreach (var vteam in view)
            {
                var team = teams[i];
                foreach (var vpseudo in vteam)
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
            int order = 1;
            foreach (var vmatch_making in view)
            {
                tournament.Matches.Add(match_manager.Create(vmatch_making, tournament_token, MatchStage.Pool, order++));
            }

            // save
            context.Entry(tournament).State = EntityState.Modified;
            context.SaveChanges();

            return tournament;
        }

        public void End(Tournament tournament)
        {
            if (tournament.State != TournamentState.Final)
                throw new Exception();

            ChampionsToSave(tournament);

            tournament.State = TournamentState.Ended;

            context.SaveChanges();
        }

        public void Score(Tournament tournament)
        {
            if (tournament.State != TournamentState.Ended)
                throw new Exception();

            ScoreToSave(tournament);

            tournament.State = TournamentState.Scored;

            context.SaveChanges();
        }

        public void EndAndScore(Tournament tournament)
        {
            if (tournament.State != TournamentState.Final)
                throw new Exception();

            ChampionsToSave(tournament);

            ScoreToSave(tournament);

            tournament.State = TournamentState.Scored;

            context.SaveChanges();
        }

        private void ChampionsToSave(Tournament tournament)
        {
            var final = tournament.Matches.FirstOrDefault(t => t.Stage == MatchStage.Final);

            if (final == null)
                throw new Exception();

            var teams = final.TeamsOfMatch.Select(t => t.Team).ToList();
            var players = teams.SelectMany(t => t.PlayersOfTeam).Select(t => t.Player).ToList();

            var goals = new List<int>
            {
                final.GoalsOfMatch.Where(t => t.PlayerId == players[0].PlayerId).First().Goals,
                final.GoalsOfMatch.Where(t => t.PlayerId == players[1].PlayerId).First().Goals,
                final.GoalsOfMatch.Where(t => t.PlayerId == players[2].PlayerId).First().Goals,
                final.GoalsOfMatch.Where(t => t.PlayerId == players[3].PlayerId).First().Goals,
            };

            var team_goals = new List<int>
            {
                goals[0] + goals[1],
                goals[2] + goals[3]
            };

            Team winner = teams[team_goals[0] > team_goals[1] ? 0 : 1];

            foreach (var player in winner.PlayersOfTeam.Select(t => t.Player))
                ++player.Champions;
        }

        private void ScoreToSave(Tournament tournament)
        {
            var teams = new List<Team>(tournament.Matches.SelectMany(t => t.TeamsOfMatch).DistinctBy(t => t.TeamId).Select(t => t.Team));

            int scale = 400;
            int coeff = 10;
            // a player is 10 times better than an other player when he has +400 elo score
            var player_win_prob = new LogisticWinProbabilityCalculator(scale, coeff);
            // a team is 10 times better than an other team when it has +800 elo score
            var team_win_prob = new LogisticWinProbabilityCalculator(scale * 2, coeff);
            // k factor is constant in this conception, but it can be imagined as player-dependant

            // 400 / 20 = 20, player gains or loses at most 20 elo score each match
            // this value must be very low, because there are many matches in a single tournament
            double all_players_k_factor = scale / 20;

            PlayerPerformanceCalculator player_perf_calc = new PlayerPerformanceCalculator(player_win_prob);

            var matches = tournament.Matches;

            var player_score_diffs = new Dictionary<int, double>();
            var players = new List<Player>(teams.SelectMany(t => t.PlayersOfTeam.Select(t => t.Player)));

            foreach (Player player in players)
                player_score_diffs.Add(player.PlayerId, 0);


            foreach (Match match in matches)
            {
                if (match.State != MatchState.Ended)
                    throw new Exception();

                List<Team> match_teams = new List<Team>(match.TeamsOfMatch.Select(t => t.Team));
                var (t1, t2) = (match_teams[0], match_teams[1]);
                var (p1, p2) = (t1.PlayersOfTeam.ElementAt(0).Player, t1.PlayersOfTeam.ElementAt(1).Player);
                var (p3, p4) = (t2.PlayersOfTeam.ElementAt(0).Player, t2.PlayersOfTeam.ElementAt(1).Player);

                var match_players = new List<Player> { p1, p2, p3, p4 };

                // get team effected performance
                double t1_effected_perf;
                if (t1.Points < t2.Points)
                    t1_effected_perf = 0;
                else if (t1.Points > t2.Points)
                    t1_effected_perf = 1;
                else
                {
                    if (match.Stage != MatchStage.Pool)
                        throw new Exception();
                    t1_effected_perf = 0.5;
                }
                var team_effected_perfs = new List<double>
                {
                    t1_effected_perf,
                    1 - t1_effected_perf
                };

                // get team elo scores
                var team_scores = new List<Double>
                {
                    p1.Score + p2.Score,
                    p3.Score + p4.Score
                };

                // get team win probabilities
                // between [0, 1]
                double t1_win_prob = team_win_prob.Get(team_scores[0], team_scores[1]);
                var team_win_probs = new List<double>
                {
                    t1_win_prob,
                    1 - t1_win_prob
                };

                // get team performances
                // between [-1, 1] (relative)
                var team_relative_perfs = new List<double>
                {
                    team_effected_perfs[0] - team_win_probs[0],
                    team_effected_perfs[1] - team_win_probs[1]
                };

                // get player goals
                var goals = new List<double>
                {
                    match.GoalsOfMatch.Where(t => t.PlayerId == p1.PlayerId).First().Goals,
                    match.GoalsOfMatch.Where(t => t.PlayerId == p2.PlayerId).First().Goals,
                    match.GoalsOfMatch.Where(t => t.PlayerId == p3.PlayerId).First().Goals,
                    match.GoalsOfMatch.Where(t => t.PlayerId == p4.PlayerId).First().Goals
                };

                // get player stats (goals + elo score)
                var stats = new List<Stats>
                {
                    new Stats { goals = goals[0], score = p1.Score },
                    new Stats { goals = goals[1], score = p2.Score },
                    new Stats { goals = goals[2], score = p3.Score },
                    new Stats { goals = goals[3], score = p4.Score }
                };

                // get player perfs
                // see PlayerPerformanceCalculator for more info
                // between [0, 1]
                var player_perfs = player_perf_calc.Get(stats);

                /*
                Debug.WriteLine(perfs);
                foreach(double b in perfs)
                    Debug.WriteLine(b);
                Debug.WriteLine("");

                Debug.WriteLine(diff_teams[0]);
                Debug.WriteLine(diff_teams[1]);
                */

                double stage_relative_coeff;
                if (match.Stage == MatchStage.Pool)
                    stage_relative_coeff = 1;
                else if (match.Stage == MatchStage.Semifinal)
                    stage_relative_coeff = 1.5;
                else
                    stage_relative_coeff = 2;

                // add player elo score delta
                for (int i = 0; i < 4; ++i)
                {
                    int player_id = match_players[i].PlayerId;
                    double team_relative_perf = team_relative_perfs[i / 2]; // [-1, 1]
                    double player_relative_perf = (2 * player_perfs[i] - 1); // [0, 1] -> [-1, 1]
                    double player_final_relative_perf = team_relative_perf / 2 + player_relative_perf / 2; // [-0.5, 0.5] + [-0.5, 0.5] = [-1, 1]
                    double player_k_factor = all_players_k_factor * stage_relative_coeff;

                    player_score_diffs[player_id] += player_k_factor * player_final_relative_perf;
                }
            }

            // add all the delta
            foreach (var pair in player_score_diffs)
            {
                var player = players.First(t => t.PlayerId == pair.Key);
                Debug.WriteLine(" v = " + pair.Value);
                player.Score += (int)pair.Value;

                context.Entry(player).State = EntityState.Modified;
            }
        }


    }
}
