﻿using babyfoot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class TournamentView
    {
        [JsonProperty(Order = 1, PropertyName = "token")]
        public String Token { get; set; }

        [JsonProperty(Order = 2, PropertyName = "date")]
        public System.DateTime CreateDate { get; set; }

        [JsonProperty(Order = 3, PropertyName = "state")]
        public String StateStr { get; set; }

        [JsonProperty(Order = 4, PropertyName = "teams")]
        public List<TournamentTeamView> Teams { get; set; }

        [JsonProperty(Order = 5, PropertyName = "matches")]
        public List<MatchView> Matches { get; set; }



        [JsonIgnore]
        public TournamentState State => (TournamentState)Enum.Parse(typeof(TournamentState), StateStr);
    }
}
