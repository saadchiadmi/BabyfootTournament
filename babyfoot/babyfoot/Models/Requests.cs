using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Models
{
    public class Requests
    {
        /*
        public static Team Winner(Match match)
        {
            var (t1, t2) = match.TeamPair;
            var (p1, p2) = t1.PlayerPair;
            var (p3, p4) = t2.PlayerPair;
            var score1 = p1.GoalsOfPlayer.First().Goals + p2.GoalsOfPlayer.First().Goals;
            var score2 = p3.GoalsOfPlayer.First().Goals + p4.GoalsOfPlayer.First().Goals;
            if (score1 > score2)
                return t1;
            else
                return t2;
        }

        public static Match Final(Tournament tournament)
        {
            return tournament.Matches.First(m => m.Stage == MatchStage.Final);
        }

        public static Team Winner(Tournament tournament)
        {
            return Winner(Final(tournament));
        }
        */
    }
}
