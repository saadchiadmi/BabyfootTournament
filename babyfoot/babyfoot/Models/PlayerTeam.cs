using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class PlayerTeam
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TeamId { get; set; }
        public int PlayerId { get; set; }

        [JsonIgnore]
        public Team Team { get; set; }
        [JsonIgnore]
        public Player Player { get; set; }
    }
}