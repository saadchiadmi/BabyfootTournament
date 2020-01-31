using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class Team
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]


        [JsonIgnore]
        public int TeamId { get; set; }

        [JsonProperty(Order = 3, PropertyName = "point")]
        public int Points { get; set; }

        [JsonIgnore]
        public ICollection<MatchTeam> MatchesOfTeam { get; set; }// = new List<MatchTeam>();

        [JsonIgnore]
        public ICollection<PlayerTeam> PlayersOfTeam { get; set; }// = new List<PlayerTeam>();

        [JsonProperty(Order = 1, PropertyName = "player1")]
        public Player Player1 => PlayerPair.Item1;

        [JsonProperty(Order = 2, PropertyName = "player2")]
        public Player Player2 => PlayerPair.Item1;

        [JsonIgnore]
        public (Player, Player) PlayerPair => (PlayersOfTeam.ElementAt(0).Player, PlayersOfTeam.ElementAt(1).Player);
    }
}
