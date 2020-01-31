using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class MatchTeam
    {
        public int TeamId { get; set; }
        public int MatchId { get; set; }

        public Team Team { get; set; }
        public Match Match { get; set; }
    }
}