using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class PlayerGoalView
    {
        [JsonProperty(Order = 1, PropertyName = "pseudo")]
        public String Pseudo { get; set; }

        [JsonProperty(Order = 2, PropertyName = "goals")]
        public int Goals { get; set; }
    }
}
