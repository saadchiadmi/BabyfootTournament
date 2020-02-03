using babyfoot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class MatchView
    {
        [JsonProperty(Order = 2, PropertyName = "token")]
        public String Token { get; set; }

        [JsonProperty(Order = 3, PropertyName = "order")]
        public int Order { get; set; }

        [JsonProperty(Order = 4, PropertyName = "stage")]
        public String StageStr { get; set; }

        [JsonProperty(Order = 5, PropertyName = "state")]
        public String StateStr { get; set; }

        [JsonProperty(Order = 6, PropertyName = "start")]
        public System.DateTime StartDate { get; set; }

        [JsonProperty(Order = 7, PropertyName = "elapsed")]
        public int ElapsedSeconds { get; set; }

        [JsonProperty(Order = 8, PropertyName = "teams")]
        public List<MatchTeamView> Teams { get; set; }



        [JsonIgnore]
        public MatchStage Stage => (MatchStage)Enum.Parse(typeof(MatchStage), StageStr);

        [JsonIgnore]
        public MatchState State => (MatchState)Enum.Parse(typeof(MatchState), StateStr);
    }
}
