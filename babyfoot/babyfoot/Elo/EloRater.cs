using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Elo
{

    public class EloRater
    {
        private IWinProbabilityCalculator pcalculator;

        public EloRater() : this(new LogisticWinProbabilityCalculator())
        {

        }

        public EloRater(IWinProbabilityCalculator pcalculator)
        {
            this.pcalculator = pcalculator;
        }

        public double Get(double score_a, double score_b, double effected_a, double k_factor)
        {
            double pa = pcalculator.Get(score_a, score_b);

            double p_diff = effected_a - pa;
            double score_diff = k_factor * p_diff;
            return score_diff;
        }

        public IWinProbabilityCalculator GetWinProbabilityCalculator()
        {
            return pcalculator;
        }

        public int GetScale()
        {
            return pcalculator.GetScale();
        }

        public void SetScale(int scale)
        {
            pcalculator.SetScale(scale);
        }
    }
}
