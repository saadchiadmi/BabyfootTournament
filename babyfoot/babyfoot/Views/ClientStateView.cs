using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class ClientStateView
    {
        [JsonProperty(Order = 1, PropertyName = "pseudos")]
        public List<String> Pseudos { get; set; }

        [JsonProperty(Order = 2, PropertyName = "tournaments")]
        public List<TournamentView> Tournaments { get; set; }
    }
}
