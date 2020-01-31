using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace babyfoot.Models
{
    public enum TournamentState
    {
        Pool, Semifinal, Final, Ended
    };

    public class Tournament
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [JsonProperty(Order = 1, PropertyName = "id")]
        public int TournamentId { get; set; }

        [JsonProperty(Order = 2, PropertyName = "token")]
        public String Token { get; set; }

        [JsonProperty(Order = 3, PropertyName = "date")]
        public System.DateTime CreateDate { get; set; }

        [JsonIgnore]
        public TournamentState State { get; set; }

        [JsonProperty(Order = 5, PropertyName = "matchs")]
        public ICollection<Match> Matches { get; set; }

        [JsonProperty(Order = 4, PropertyName = "finish")]
        public bool Finish => State == TournamentState.Ended;

        [JsonProperty(Order = 6, PropertyName = "teams")]
        public IEnumerable<Team> Teams => Matches.SelectMany(t => t.TeamsOfMatch).Select(t => t.Team);
    }
}