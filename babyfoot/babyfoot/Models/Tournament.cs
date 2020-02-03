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

        public int TournamentId { get; set; }

        public String Token { get; set; }

        public System.DateTime CreateDate { get; set; }



        public TournamentState State { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}