using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace babyfoot.Models
{
    public enum MatchState
    {
        NotStarted,
        InProgress,
        Ended
    };
    public enum MatchStage
    {
        [EnumMember(Value = "poule")]
        Pool,
        [EnumMember(Value = "semifinal")]
        Semifinal,
        [EnumMember(Value = "final")]
        Final
    };

    public class Match
    {

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        [JsonProperty(Order = 1, PropertyName = "id")]
        public int MatchId { get; set; }

        [JsonIgnore]
        public int TournamentId { get; set; }

        [JsonProperty(Order = 2, PropertyName = "token")]
        public String Token { get; set; }

        [JsonProperty(Order = 5, PropertyName = "ordre")]
        public int Order { get; set; }


        [JsonProperty(Order = 6, PropertyName = "niveau")]
        public MatchStage Stage { get; set; }

        [JsonIgnore]
        public MatchState State { get; set; }

        [JsonProperty(Order = 5, PropertyName = "start")]
        public System.DateTime StartDate { get; set; }

        [JsonIgnore]
        public Tournament Tournament { get; set; }

        [JsonIgnore]
        public ICollection<MatchTeam> TeamsOfMatch { get; set; }

        [JsonIgnore]
        public ICollection<PlayerGoal> GoalsOfMatch { get; set; }

        [JsonProperty(Order = 3, PropertyName = "team1")]
        public Team Team1 => TeamPair.Item1;

        [JsonProperty(Order = 4, PropertyName = "team2")]
        public Team Team2 => TeamPair.Item2;

        [JsonProperty(Order = 7, PropertyName = "scoreTeam1Player1")]
        public int ScoreTeam1Player1 => Team1.Player1.Goals;

        [JsonProperty(Order = 8, PropertyName = "scoreTeam1Player2")]
        public int ScoreTeam1Player2 => Team1.Player2.Goals;

        [JsonProperty(Order = 9, PropertyName = "scoreTeam2Player1")]
        public int ScoreTeam2Player1 => Team2.Player1.Goals;

        [JsonProperty(Order = 10, PropertyName = "scoreTeam2Player2")]
        public int ScoreTeam2Player2 => Team2.Player2.Goals;

        [JsonIgnore]
        public (Team, Team) TeamPair { get { return (TeamsOfMatch.ElementAt(0).Team, TeamsOfMatch.ElementAt(1).Team); } }
    }
}