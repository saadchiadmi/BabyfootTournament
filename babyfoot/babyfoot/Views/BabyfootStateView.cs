using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class BabyfootStateView
    {
        [JsonProperty(Order = 1, PropertyName = "players")]
        public List<PlayerView> Players { get; set; }

        [JsonProperty(Order = 2, PropertyName = "tournaments")]
        public List<TournamentView> Tournaments { get; set; }
    }
}
