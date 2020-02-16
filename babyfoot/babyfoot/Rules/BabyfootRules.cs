using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Rules
{
    public static class BabyfootRules
    {
        public static int PoolMaxGoals { get; } = 5;

        public static int WinPoints { get; } = 3;

        public static int DrawPoints { get; } = 1;

        public static int LossPoints { get; } = 0;
    }
}
