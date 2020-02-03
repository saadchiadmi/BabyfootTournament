﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class MatchTeamView
    {
        [JsonProperty(Order = 1, PropertyName = "points")]
        public int Points { get; set; }

        [JsonProperty(Order = 2, PropertyName = "players")]
        public List<PlayerGoalView> Players { get; set; }
    }
}
