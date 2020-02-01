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

namespace babyfoot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly BabyfootDbContext context;

        public TournamentsController(BabyfootDbContext context)
        {
            this.context = context;
        }

        
        // GET: api/tournaments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournament()
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

            return tournaments;
        }

        // GET: api/tournaments/token
        [HttpGet("{token}")]
        public async Task<ActionResult<Tournament>> GetTournament(String token)
        {
            var tournament = await context.Tournaments.Where(t => t.Token.Equals(token))
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


            return tournament;
        }

        // POST: api/tournaments
        [HttpPost]
        public void PostTournament([FromBody] Tournament tournament)
        {
            context.Tournaments.Add(tournament);
            context.SaveChanges();
        }
    }
}
