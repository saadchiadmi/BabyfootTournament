using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class PlayerGoal
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int Goals { get; set; }

        public Player Player { get; set; }
        public Match Match { get; set; }
    }
}