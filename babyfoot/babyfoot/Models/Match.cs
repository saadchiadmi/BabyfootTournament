using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace babyfoot.Models
{
    public class Match
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int TournamentID { get; set; }
        public string token { get; set; }
        public string start { get; set; }
        public Boolean finish { get { return true; } }
        public string time { get; set; }
        public int ordre { get; set; }
        public string niveau { get; set; }
        public Teams team1{get {return MatchTeams.Count() == 0 ? null : MatchTeams.ElementAt(0).Team;} }
        public Teams team2{get {return MatchTeams.Count() == 0 ? null : MatchTeams.ElementAt(1).Team; } }
        public Teams winner { get {
                return finish? (scoreTeam1Player1 + scoreTeam1Player2) > (scoreTeam2Player1 + scoreTeam2Player2) ? team1 : team2 : null;
            } }
        public int scoreTeam1Player1 { get {
                if (team1 == null) return 0;
                IEnumerable<MatchGoals> listGoals = MatchGoals.Where(m => m.PlayerID == team1.TeamPlayers.ElementAt(0).PlayerID);
                return listGoals.Count()==0 ? 0 : listGoals.ElementAt(0).goals;
            } }
        public int scoreTeam1Player2 { get {
                if (team1 == null) return 0;
                IEnumerable<MatchGoals> listGoals = MatchGoals.Where(m => m.PlayerID == team1.TeamPlayers.ElementAt(1).PlayerID);
                return listGoals.Count() == 0 ? 0 : listGoals.ElementAt(0).goals;
            } }
        public int scoreTeam2Player1 { get {
                if (team2 == null) return 0;
                IEnumerable<MatchGoals> listGoals = MatchGoals.Where(m => m.PlayerID == team2.TeamPlayers.ElementAt(0).PlayerID);
                return listGoals.Count() == 0 ? 0 : listGoals.ElementAt(0).goals;
            } }
        public int scoreTeam2Player2 { get {
                if (team2 == null) return 0;
                IEnumerable<MatchGoals> listGoals = MatchGoals.Where(m => m.PlayerID == team2.TeamPlayers.ElementAt(1).PlayerID);
                return listGoals.Count() == 0 ? 0 : listGoals.ElementAt(0).goals;
            } }

        [JsonIgnore]
        public virtual Tournament Tournament { get; set; } = new Tournament();
        [JsonIgnore]
        public virtual ICollection<MatchGoals> MatchGoals { get; set; } = new List<MatchGoals>();
        [JsonIgnore]
        public virtual ICollection<MatchTeams> MatchTeams { get; set; } = new List<MatchTeams>();
    }
}
