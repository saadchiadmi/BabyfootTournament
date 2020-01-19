using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace babyfoot.Models
{
    public class Teams
    {
        public Players _player1;
        public Players _player2;
        [Key]
        public int ID { get; set; }
        public int point { get; set; }
        [JsonIgnore]
        public virtual ICollection<MatchTeams> MatchTeams { get; set; } = new List<MatchTeams>();
        //[JsonIgnore]
        public virtual ICollection<TeamPlayers> TeamPlayers { get; set; } = new List<TeamPlayers>();
        public Players player1 { get { return TeamPlayers.Count()==0 ? null : TeamPlayers.Select(tp => tp.Player).ElementAt(0); }
            //set { TeamPlayers.Add(new TeamPlayers { TeamID = ID, PlayerID = value.ID }); }
            set { _player1 = value; }
        }
        public Players player2 { get { return TeamPlayers.Count() >= 1 ? null : TeamPlayers.Select(tp => tp.Player).ElementAt(1); }
            //set { TeamPlayers.Add(new TeamPlayers { TeamID = ID, PlayerID = value.ID }); }
            set { _player2 = value; }
        }
    }
}
