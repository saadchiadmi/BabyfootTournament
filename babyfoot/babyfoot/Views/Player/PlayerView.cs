using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class PlayerView
    {
        [JsonProperty(Order = 1, PropertyName = "pseudo")]
        public String Pseudo { get; set; }

        [JsonProperty(Order = 2, PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(Order = 3, PropertyName = "goals")]
        public int Goals { get; set; }

        [JsonProperty(Order = 4, PropertyName = "champions")]
        public int Champions { get; set; }
    }
}
