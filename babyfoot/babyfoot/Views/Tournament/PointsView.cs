using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class MatchScoresView : List<MatchTeamScoreView>
    {
        
    }

    public class MatchTeamScoreView
    {
        [JsonProperty(Order = 1, PropertyName = "points")]
        public int Points { get; set; }

        [JsonProperty(Order = 2, PropertyName = "pseudos")]
        public List<MatchPlayerView> Players { get; set; }
    }
}
