using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class IdentifiedMatchView
    {
        [JsonProperty(Order = 1, PropertyName = "tournament")]
        public String TournamentToken;

        [JsonProperty(Order = 2, PropertyName = "match")]
        public MatchView Match;
    }
}
