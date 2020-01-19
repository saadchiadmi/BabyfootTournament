using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace babyfoot.Models
{
    public class MatchTeams
    {
        //[Required]
        public int MatchID { get; set; }
        //[Required]
        public int TeamID { get; set; }
        //[ForeignKey("Match"), IgnoreDataMember, NotMapped]
        public virtual Match Match { get; set; }
        //[ForeignKey("Teams"), IgnoreDataMember]
        public virtual Teams Team { get; set; }

        
    }
}
