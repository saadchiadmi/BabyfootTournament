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
        private readonly ConnectionDBClass _context;

        public TournamentsController(ConnectionDBClass context)
        {
            _context = context;
        }

        // GET: api/Tournaments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournament()
        {
            return await _context.Tournament.Include(t => t.Matches)
                                                .ThenInclude(m => m.MatchTeams)
                                                    .ThenInclude(mt => mt.Team)
                                                        .ThenInclude(t => t.TeamPlayers)
                                                            .ThenInclude(tp => tp.Player)
                                            .Include(t => t.Matches)
                                                .ThenInclude(m => m.MatchGoals)
                        .ToListAsync();
        }

        // GET: api/Tournaments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournament(int id)
        {
            var tournament = await _context.Tournament.Include(t => t.Matches)
                                                .ThenInclude(m => m.MatchTeams)
                                                    .ThenInclude(mt => mt.Team)
                                                        .ThenInclude(t => t.TeamPlayers)
                                                            .ThenInclude(tp => tp.Player)
                                                .Include(t => t.Matches)
                                                .ThenInclude(m => m.MatchGoals)
                                    .FirstOrDefaultAsync(i => i.ID == id);

            if (tournament == null)
            {
                return NotFound();
            }

            return tournament;
        }

        // PUT: api/Tournaments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournament(int id, Tournament tournament)
        {
            if (id != tournament.ID)
            {
                return BadRequest();
            }

            _context.Entry(tournament).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(id))
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

        // POST: api/Tournaments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public void PostTournament([FromBody] Tournament tournament)
        {
            _context.Tournament.Add(tournament);
            _context.SaveChanges();
        }

        // DELETE: api/Tournaments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tournament>> DeleteTournament(int id)
        {
            var tournament = await _context.Tournament.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }

            _context.Tournament.Remove(tournament);
            await _context.SaveChangesAsync();

            return tournament;
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournament.Any(e => e.ID == id);
        }
    }
}
