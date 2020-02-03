using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class Team
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int TeamId { get; set; }

        public int Points { get; set; }



        public ICollection<MatchTeam> MatchesOfTeam { get; set; }

        public ICollection<PlayerTeam> PlayersOfTeam { get; set; }
    }
}
