using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class Player
    {
        [JsonIgnore]
        public int PlayerId { get; set; }

        [JsonProperty(Order = 1, PropertyName = "pseudo")]
        public String Pseudo { get; set; }

        [JsonProperty(Order = 2, PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(Order = 3, PropertyName = "goals")]
        public int Goals { get; set; }

        [JsonProperty(Order = 4, PropertyName = "champions")]
        public int Champions { get; set; }

        [JsonIgnore]
        public ICollection<PlayerTeam> TeamsOfPlayer { get; set; }// = new List<PlayerTeam>();
        [JsonIgnore]
        public ICollection<PlayerGoal> GoalsOfPlayer { get; set; }// = new List<PlayerGoal>();
    }
}
