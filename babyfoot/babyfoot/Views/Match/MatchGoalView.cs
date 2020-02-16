using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class MatchGoalView
    {
        [JsonProperty(Order = 1, PropertyName = "token")]
        public String Token { get; set; }

        [JsonProperty(Order = 2, PropertyName = "pseudo")]
        public String Pseudo { get; set; }
    }
}
