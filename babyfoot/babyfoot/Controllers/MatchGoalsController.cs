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
    public class MatchGoalsController : ControllerBase
    {
        private readonly ConnectionDBClass _context;

        public MatchGoalsController(ConnectionDBClass context)
        {
            _context = context;
        }

        // GET: api/MatchGoals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchGoals>>> GetMatchGoals()
        {
            return await _context.MatchGoals.Include(t => t.Match).Include(t => t.Player).ToListAsync();
        }

        // GET: api/MatchGoals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchGoals>> GetMatchGoals(int id)
        {
            var matchGoals = await _context.MatchGoals.FindAsync(id);

            if (matchGoals == null)
            {
                return NotFound();
            }

            return matchGoals;
        }

        // PUT: api/MatchGoals/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatchGoals(int id, MatchGoals matchGoals)
        {
            if (id != matchGoals.ID)
            {
                return BadRequest();
            }

            _context.Entry(matchGoals).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchGoalsExists(id))
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

        // POST: api/MatchGoals
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MatchGoals>> PostMatchGoals(MatchGoals matchGoals)
        {
            _context.MatchGoals.Add(matchGoals);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatchGoals", new { id = matchGoals.ID }, matchGoals);
        }

        // DELETE: api/MatchGoals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MatchGoals>> DeleteMatchGoals(int id)
        {
            var matchGoals = await _context.MatchGoals.FindAsync(id);
            if (matchGoals == null)
            {
                return NotFound();
            }

            _context.MatchGoals.Remove(matchGoals);
            await _context.SaveChangesAsync();

            return matchGoals;
        }

        private bool MatchGoalsExists(int id)
        {
            return _context.MatchGoals.Any(e => e.ID == id);
        }
    }
}
