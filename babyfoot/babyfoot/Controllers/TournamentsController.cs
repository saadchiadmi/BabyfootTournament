using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using babyfoot;
using babyfoot.Models;
using System.Net.Http;
using babyfoot.Views;
using babyfoot.Elo;
using static babyfoot.Elo.PlayerPerformanceCalculator;
using System.Diagnostics;

namespace babyfoot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly BabyfootDbContext context;
        private readonly BabyfootWebInterface webInterface;

        public TournamentsController(BabyfootDbContext context)
        {
            this.context = context;
            this.webInterface = new BabyfootWebInterface(context);
        }

        
        // GET: api/Tournaments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentView>>> GetTournaments()
        {
            var tournaments = await context.Tournaments
                .Include(t => t.Matches)
                    .ThenInclude(t => t.TeamsOfMatch)
                        .ThenInclude(t => t.Team)
                            .ThenInclude(t => t.PlayersOfTeam)
                                .ThenInclude(t => t.Player)
                .Include(t => t.Matches)
                    .ThenInclude(t => t.GoalsOfMatch)
                        .ToListAsync();

            var views = new List<TournamentView>(tournaments.Select(t => webInterface.View(t)));

            return views;
        }

        // GET: api/Tournaments/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<TournamentView>> GetTournament(String token)
        {
            var tournament = await context.Tournaments
                .Where(t => t.Token.Equals(token))
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                            .FirstOrDefaultAsync();

            if (tournament == null)
                return NotFound();

            var view = webInterface.View(tournament);

            return view;
        }

        // PUT: api/Tournaments/{token}
        [HttpPut("{token}")]
        public IActionResult PutTournament(String token, TournamentView view)
        {
            if (!token.Equals(view.Token))
                return BadRequest();

            using (var transaction = context.Database.BeginTransaction())
            {
                var tournament = webInterface.ModifyAsync(view).Result;
                transaction.Commit();
            }

            return NoContent();
        }

        [HttpPut("{token}/stats")]
        public async Task<IActionResult> PutStats(String token)
        {
            var tournament = await context.Tournaments
                .Where(t => t.Token.Equals(token))
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                            .FirstOrDefaultAsync();

            if (tournament == null)
                return NotFound();

            if (!(tournament.State == TournamentState.Final || tournament.State == TournamentState.Ended))
                return BadRequest();

            using (var transaction = context.Database.BeginTransaction())
            {
                tournament.State = TournamentState.Ended;
                var teams = new List<Team>(tournament.Matches.SelectMany(t => t.TeamsOfMatch).DistinctBy(t => t.TeamId).Select(t => t.Team));

                EloRater team_rater = new EloRater(new LogisticWinProbabilityCalculator(400 * 2, 10));
                PlayerPerformanceCalculator player_perf_calc = new PlayerPerformanceCalculator(team_rater);

                var matches = tournament.Matches;
                double team_k_factor = 50;

                var win_prob = team_rater.GetWinProbabilityCalculator();
                var player_score_diffs = new Dictionary<int, double>();
                var players = new List<Player>(teams.SelectMany(t => t.PlayersOfTeam.Select(t => t.Player)));

                foreach (Player player in players)
                    player_score_diffs.Add(player.PlayerId, 0);


                foreach (Match match in matches)
                {
                    List<Team> match_teams = new List<Team>(match.TeamsOfMatch.Select(t => t.Team));
                    var (t1, t2) = (match_teams[0], match_teams[1]);
                    var (p1, p2) = (t1.PlayersOfTeam.ElementAt(0).Player, t1.PlayersOfTeam.ElementAt(1).Player);
                    var (p3, p4) = (t2.PlayersOfTeam.ElementAt(0).Player, t2.PlayersOfTeam.ElementAt(1).Player);

                    var match_players = new List<Player> { p1, p2, p3, p4 };


                    double effected_t1;
                    if (t1.Points < t2.Points)
                        effected_t1 = 0;
                    else if (t1.Points > t2.Points)
                        effected_t1 = 1;
                    else
                        effected_t1 = 0.5;


                    double score_t1 = p1.Score + p2.Score;
                    double score_t2 = p3.Score + p4.Score;
                    var diff_teams = new List<double>
                    {
                        team_rater.Get(score_t1, score_t2, effected_t1, team_k_factor),
                        team_rater.Get(score_t2, score_t1, 1 - effected_t1, team_k_factor)
                    };

                    var goals = new List<double>
                    {
                        match.GoalsOfMatch.Where(t => t.PlayerId == p1.PlayerId).First().Goals,
                        match.GoalsOfMatch.Where(t => t.PlayerId == p2.PlayerId).First().Goals,
                        match.GoalsOfMatch.Where(t => t.PlayerId == p3.PlayerId).First().Goals,
                        match.GoalsOfMatch.Where(t => t.PlayerId == p4.PlayerId).First().Goals
                    };

                    var stats = new List<Stats>
                    {
                        new Stats { goals = goals[0], score = p1.Score },
                        new Stats { goals = goals[1], score = p2.Score },
                        new Stats { goals = goals[2], score = p3.Score },
                        new Stats { goals = goals[3], score = p4.Score }
                    };
                    var perfs = player_perf_calc.Get(stats);
                
                    Debug.WriteLine(perfs);
                    foreach(double b in perfs)
                        Debug.WriteLine(b);
                    Debug.WriteLine("");

                    Debug.WriteLine(diff_teams[0]);
                    Debug.WriteLine(diff_teams[1]);

                    for (int i = 0; i < 4; ++i)
                    {
                        var diff_player = diff_teams[i / 2] / 2;
                        player_score_diffs[match_players[i].PlayerId] += diff_player + (Math.Abs(diff_player) * perfs[i]);
                    }
                }

                foreach(var pair in player_score_diffs)
                {
                    var player = players.First(t => t.PlayerId == pair.Key);
                    Debug.WriteLine(" v = " + pair.Value);
                    player.Score += (int)pair.Value;

                    context.Entry(player).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();
                transaction.Commit();
            }

            return NoContent();
        }

        // PUT: api/Tournaments
        [HttpPut]
        public void PutTournaments(BabyfootStateView view)
        {
            // TODO
            //var known = context.Players.Where(t => view.Players.Select(t => t.Pseudo).Contains(t.Pseudo));


            //context.SaveChanges();
        }

        // POST: api/Tournaments
        [HttpPost]
        public IActionResult PostTournament(TournamentView view)
        {
            if (context.Tournaments.Any(t => t.Token.Equals(view.Token)))
                return BadRequest();

            using(var transaction = context.Database.BeginTransaction())
            {
                var tournament = webInterface.Create(view);
                transaction.Commit();
            }

            return NoContent();
        }


        // POST: api/Tournaments/{token}/matches
        [HttpPost("{token}/matches")]
        public ActionResult<MatchView> PostMatch(String token, MatchView view)
        {
            var match_token = view.Token;
            if (!context.Tournaments.Any(t => t.Token.Equals(token)))
                return BadRequest();
            if (context.Matches.Any(t => t.Token.Equals(match_token)))
                return BadRequest();

            using (var transaction = context.Database.BeginTransaction())
            {
                var match = webInterface.CreateAsync(view, token).Result;
                transaction.Commit();
            }

            var task = CreatedAtAction("PostMatch", match_token, view);

            return task;
        }
    }
}
