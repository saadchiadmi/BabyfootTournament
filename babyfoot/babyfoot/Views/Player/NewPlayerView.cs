using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class NewPlayerView
    {
        [JsonProperty(Order = 1, PropertyName = "pseudo")]
        public String Pseudo { get; set; }

        [JsonProperty(Order = 2, PropertyName = "score")]
        public int Score { get; set; }
    }
}