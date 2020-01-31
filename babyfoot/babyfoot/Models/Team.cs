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
        public ICollection<MatchTeam> MatchesOfTeam { get; set; }

        [JsonIgnore]
        public ICollection<PlayerTeam> PlayersOfTeam { get; set; }

        [JsonProperty(Order = 1, PropertyName = "player1")]
        public Player Player1 { get { return PlayerPair.Item1; } set { Player1 = value; } }

        [JsonProperty(Order = 2, PropertyName = "player2")]

        public Player Player2 { get { return PlayerPair.Item2; } set { Player2 = value; } }

        [JsonIgnore]
        public (Player, Player) PlayerPair => (PlayersOfTeam.ElementAt(0).Player, PlayersOfTeam.ElementAt(1).Player);
    }
}
