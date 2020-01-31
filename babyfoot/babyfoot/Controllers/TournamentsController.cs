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

        /*
        // GET: api/Tournaments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournament()
        {
            var tournaments = await context.Tournaments.ToListAsync();

            return tournaments;
        }
        */

        // GET: api/tournaments/token
        [HttpGet("{token}")]
        public async Task<ActionResult<Tournament>> GetTournament(String token)
        {
            var tournament = await context.Tournaments.FirstOrDefaultAsync(t => t.Token.Equals(token));

            if (tournament == null)
            {
                return NotFound();
            }

            return tournament;
        }

        /*
        // PUT: api/tournaments/token
        [HttpPut("{token}")]
        public async Task<IActionResult> PutTournament(String token, Tournament tournament)
        {
            if (!token.Equals(tournament.Token))
                return BadRequest();

            context.Entry(tournament).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Tournaments.Any(t => t.Token == token))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */

        // POST: api/tournaments
        [HttpPost]
        public void PostTournament([FromBody] Tournament tournament)
        {
            context.Tournaments.Add(tournament);
            context.SaveChanges();
        }
    }
}
