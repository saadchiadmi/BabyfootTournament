using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Elo
{
    // based on logistic function : 1 / (1 + e^x)
    // For every difference of <scale> points, the team/player with the higher score is <coeff> times as likely to win as the other team/player.
    public class LogisticWinProbabilityCalculator : IWinProbabilityCalculator
    {
        private int scale;
        private int coeff;

        public LogisticWinProbabilityCalculator() : this(400, 10)
        {

        }

        public LogisticWinProbabilityCalculator(int scale, int coeff)
        {
            this.scale = scale;
            this.coeff = coeff;
        }

        public int GetScale()
        {
            return scale;
        }

        public double Get(double ra, double rb)
        {
            return 1.0 / (1 + (Math.Pow(coeff, 1.0f * Math.Abs(rb - ra) / scale)));
        }
    }
}
