using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using babyfoot.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace babyfoot
{
    public class Tournament
    {
        
        [Key]
        public int ID { get; set; }
        public string date { get; set; }
        public string token { get; set; }

        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

        public Boolean finish
        {
            get
            {
                foreach (Match match in Matches)
                {
                    if (match.niveau.Equals("final") && match.finish)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public HashSet<int> teamsid
        {
            get
            {
                return Matches.SelectMany(m => m.MatchTeams).Select(mt => mt.Team.ID).ToHashSet();
            }
        }
        public Teams winner
        {
            get
            {
                return Matches.Where(m=>m.niveau.Equals("final")).Select(m=>m.winner).FirstOrDefault();
            }
        }

    }
}
