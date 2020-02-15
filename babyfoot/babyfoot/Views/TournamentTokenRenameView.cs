using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class TournamentTokenRenameView
    {
        [JsonProperty(Order = 1, PropertyName = "initial")]
        public String Initial { get; set; }

        [JsonProperty(Order = 2, PropertyName = "token")]
        public String Token { get; set; }
    }
}
