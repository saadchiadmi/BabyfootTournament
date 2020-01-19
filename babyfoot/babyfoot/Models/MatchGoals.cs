using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Models
{
    public class MatchGoals
    {
        [Key]
        public int ID { get; set; }
        public int MatchID { get; set; }
        public int PlayerID { get; set; }
        public int goals { get; set; }

        public virtual Match Match { get; set; } = new Match();
        public virtual Players Player { get; set; } = new Players();
    }
}
