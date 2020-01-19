using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace babyfoot.Models
{
    public class TeamPlayers
    {
        public int PlayerID { get; set; }
        public int TeamID { get; set; }

        [JsonIgnore]
        public virtual Players Player { get; set; }
        [JsonIgnore]
        public virtual Teams Team { get; set; }
    }
}
