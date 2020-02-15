using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Rules
{
    public static class BabyfootElo
    {
        public static int Scale { get; } = 400;

        public static int Coeff { get; } = 10;

        public static int Initial { get; } = 1500;
    }
}
