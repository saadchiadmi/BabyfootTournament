using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Elo
{
    public interface IWinProbabilityCalculator
    {
        /*
         * ra   : rating of a
         * rb   : rating of b
         * 
         * returns the probability of a to win
         * 
         * p(ra, rb) = 1 - p(rb, ra)
         * 0 <= p(ra, rb) <= 1
         * 
         */
        double Get(double ra, double rb);

        int GetScale();

        void SetScale(int scale);
    }
}
