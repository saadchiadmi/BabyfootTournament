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
    public class PlayersController : ControllerBase
    {
        private readonly ConnectionDBClass _context;

        public PlayersController(ConnectionDBClass context)
        {
            _context = context;
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Players>>> GetPlayers()
        {
            return await _context.Players.Include(t => t.MatchGoals)
                                        .Include(t => t.TeamPlayers)
                                            .ThenInclude(tp=>tp.Team)
                                            .ThenInclude(t => t.MatchTeams)
                                            .ThenInclude(t => t.Match)
                                        .ToListAsync();
        }

        // GET: api/Players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Players>> GetPlayers(int id)
        {
            var players = await _context.Players.Include(t => t.MatchGoals)
                                        .Include(t => t.TeamPlayers)
                                            .ThenInclude(tp => tp.Team)
                                            .ThenInclude(t => t.MatchTeams)
                                            .ThenInclude(t => t.Match)
                                    .FirstOrDefaultAsync(i => i.ID == id);

            if (players == null)
            {
                return NotFound();
            }

            return players;
        }

        // PUT: api/Players/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayers(int id, Players players)
        {
            if (id != players.ID)
            {
                return BadRequest();
            }

            _context.Entry(players).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayersExists(id))
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

        // POST: api/Players
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Players>> PostPlayers(Players players)
        {
            _context.Players.Add(players);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayers", new { id = players.ID }, players);
        }

        // DELETE: api/Players/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Players>> DeletePlayers(int id)
        {
            var players = await _context.Players.FindAsync(id);
            if (players == null)
            {
                return NotFound();
            }

            _context.Players.Remove(players);
            await _context.SaveChangesAsync();

            return players;
        }

        private bool PlayersExists(int id)
        {
            return _context.Players.Any(e => e.ID == id);
        }
    }
}
