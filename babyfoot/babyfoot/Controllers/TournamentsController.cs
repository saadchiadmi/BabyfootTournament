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
            {
                return NotFound();
            }

            var view = webInterface.View(tournament);

            return view;
        }

        // PUT: api/Tournaments/{token}
        [HttpPut("{token}")]
        public IActionResult PutTournament(String token, TournamentView view)
        {
            if (!token.Equals(view.Token))
                return BadRequest();

            var tournament = webInterface.ModifyAsync(view).Result;

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

            var tournament = webInterface.CreateAsync(view).Result;

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

            var match = webInterface.CreateAsync(view, token).Result;

            var task = CreatedAtAction("PostMatch", match_token, view);

            return task;
        }
    }
}
