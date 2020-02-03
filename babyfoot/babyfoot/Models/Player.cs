using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class Player
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        public String Pseudo { get; set; }

        public int Score { get; set; }

        public int Goals { get; set; }

        public int Champions { get; set; }



        public ICollection<PlayerTeam> TeamsOfPlayer { get; set; }

        public ICollection<PlayerGoal> GoalsOfPlayer { get; set; }
    }
}
