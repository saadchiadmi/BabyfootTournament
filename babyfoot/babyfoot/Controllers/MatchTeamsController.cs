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
    public class MatchTeamsController : ControllerBase
    {
        private readonly ConnectionDBClass _context;

        public MatchTeamsController(ConnectionDBClass context)
        {
            _context = context;
        }

        // GET: api/MatchTeams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchTeams>>> GetMatchTeams()
        {
            return await _context.MatchTeams.Include(t => t.Match).Include(t => t.Team).ToListAsync();
        }

        // GET: api/MatchTeams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchTeams>> GetMatchTeams(int id)
        {
            var matchTeams = await _context.MatchTeams.FindAsync(id);

            if (matchTeams == null)
            {
                return NotFound();
            }

            return matchTeams;
        }

        // PUT: api/MatchTeams/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatchTeams(int id, MatchTeams matchTeams)
        {
            if (id != matchTeams.MatchID)
            {
                return BadRequest();
            }

            _context.Entry(matchTeams).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchTeamsExists(id))
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

        // POST: api/MatchTeams
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MatchTeams>> PostMatchTeams(MatchTeams matchTeams)
        {
            _context.MatchTeams.Add(matchTeams);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MatchTeamsExists(matchTeams.MatchID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMatchTeams", new { id = matchTeams.MatchID }, matchTeams);
        }

        // DELETE: api/MatchTeams/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MatchTeams>> DeleteMatchTeams(int id)
        {
            var matchTeams = await _context.MatchTeams.FindAsync(id);
            if (matchTeams == null)
            {
                return NotFound();
            }

            _context.MatchTeams.Remove(matchTeams);
            await _context.SaveChangesAsync();

            return matchTeams;
        }

        private bool MatchTeamsExists(int id)
        {
            return _context.MatchTeams.Any(e => e.MatchID == id);
        }
    }
}
