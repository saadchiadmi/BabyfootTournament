using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using babyfoot.Models;

namespace babyfoot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly BabyfootDbContext context;

        public MatchesController(BabyfootDbContext context)
        {
            this.context = context;
        }

        /*
        // GET: api/matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatch()
        {
            return await context.Matches.Include(u => u.GoalsOfMatch)
                                        .Include(u => u.TeamsOfMatch)
                                            .ThenInclude(mt => mt.Team)
                                            .ThenInclude(t => t.PlayersOfTeam)
                                            .ThenInclude(t => t.Player)
                .ToListAsync();
        }
        */

        // GET: api/matches/token
        [HttpGet("{token}")]
        public async Task<ActionResult<Match>> GetMatch(String token)
        {
            var match = await context.Matches
                .Include(u => u.GoalsOfMatch)
                .Include(u => u.TeamsOfMatch)
                    .ThenInclude(mt => mt.Team)
                    .ThenInclude(t => t.PlayersOfTeam)
                    .ThenInclude(t => t.Player)
                .FirstOrDefaultAsync();

            if (match == null)
            {
                return NotFound();
            }

            return match;
        }

        // PUT: api/Matches/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{token}")]
        public async Task<IActionResult> PutMatch(String token, Match match)
        {
            if (!token.Equals(match.Token))
                return BadRequest();

            context.Entry(match).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Matches.Any(e => e.Token.Equals(token)))
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

        // POST: api/Matches
        [HttpPost]
        public async Task<ActionResult<Match>> PostMatch(Match match)
        {
            context.Matches.Add(match);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetMatch", match.Token, match);
        }

    }
}
