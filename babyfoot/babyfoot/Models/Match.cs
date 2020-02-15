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
        Pause,
        Ended
    };
    public enum MatchStage
    {
        Pool,
        Semifinal,
        Final
    };

    public class Match
    {

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int MatchId { get; set; }

        public int TournamentId { get; set; }

        public String Token { get; set; }

        public int Order { get; set; }

        public MatchStage Stage { get; set; }

        public MatchState State { get; set; }

        public int ElapsedSeconds { get; set; }

        public System.DateTime? StartDate { get; set; }

        public System.DateTime? RestartDate { get; set; }



        public Tournament Tournament { get; set; }

        public ICollection<MatchTeam> TeamsOfMatch { get; set; }

        public ICollection<PlayerGoal> GoalsOfMatch { get; set; }
    }
}