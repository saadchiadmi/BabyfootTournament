using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace babyfoot.Models
{
    public class Players
    {
        [Key]
        public int ID { get; set; }
        public string pseudo { get; set; }
        public int score { get; set; }
        public int goals { get { return MatchGoals.Select(m => m.goals).Sum(); } }
        public int champions
        {
            get
            {
                IEnumerable<Teams> tp = TeamPlayers.Select(tp => tp.Team);
                IEnumerable<Match> mt = tp.SelectMany(t => t.MatchTeams.Select(u => u.Match));
                IEnumerable<Match> mtw = mt == null?null : mt.Where(m =>m.niveau.Equals("final") &&
                                                                 m.winner.TeamPlayers.Select(tp => tp.PlayerID).Contains(ID));
                return mtw.Count();
            }
        }
        
        [JsonIgnore]
        public virtual ICollection<MatchGoals> MatchGoals { get; set; } = new List<MatchGoals>();
        [JsonIgnore]
        public virtual ICollection<TeamPlayers> TeamPlayers { get; set; } = new List<TeamPlayers>();


        
    }
}
