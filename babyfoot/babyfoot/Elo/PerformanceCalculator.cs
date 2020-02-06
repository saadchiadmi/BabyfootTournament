using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Elo
{
    public class PlayerPerformanceCalculator
    {
        private EloRater rater;
        private IWinProbabilityCalculator win_prob;

        public PlayerPerformanceCalculator(EloRater rater)
        {
            this.rater = rater;
            this.win_prob = rater.GetWinProbabilityCalculator();
        }

        public struct Stats
        {
            public double goals;
            public double score;
        }

        public List<double> Get(List<Stats> players)
        {
            List<Double> perf = new List<double>();
            int i = 0;
            foreach(var player in players)
            {
                double sum = 0;
                int j = 0;
                foreach(var other in players)
                {
                    if (i == j)
                        continue;
                    if ((player.goals + other.goals) == 0)
                        continue;
                    sum += (player.goals / (player.goals + other.goals)) - win_prob.Get(player.score, other.score);
                    ++j;
                }
                double average_player_perf = sum / (players.Count() - 1);
                perf.Add(average_player_perf);
                ++i;
            }
            return perf;
        }
    }
}
