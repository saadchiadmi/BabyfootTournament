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
    public class TeamPlayersController : ControllerBase
    {
        private readonly ConnectionDBClass _context;

        public TeamPlayersController(ConnectionDBClass context)
        {
            _context = context;
        }

        // GET: api/TeamPlayers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamPlayers>>> GetTeamPlayers()
        {
            return await _context.TeamPlayers.Include(t => t.Team).Include(t => t.Player).ToListAsync();
        }

        // GET: api/TeamPlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamPlayers>> GetTeamPlayers(int id)
        {
            var teamPlayers = await _context.TeamPlayers.FindAsync(id);

            if (teamPlayers == null)
            {
                return NotFound();
            }

            return teamPlayers;
        }

        // PUT: api/TeamPlayers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamPlayers(int id, TeamPlayers teamPlayers)
        {
            if (id != teamPlayers.TeamID)
            {
                return BadRequest();
            }

            _context.Entry(teamPlayers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamPlayersExists(id))
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

        // POST: api/TeamPlayers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TeamPlayers>> PostTeamPlayers(TeamPlayers teamPlayers)
        {
            _context.TeamPlayers.Add(teamPlayers);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TeamPlayersExists(teamPlayers.TeamID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTeamPlayers", new { id = teamPlayers.TeamID }, teamPlayers);
        }

        // DELETE: api/TeamPlayers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TeamPlayers>> DeleteTeamPlayers(int id)
        {
            var teamPlayers = await _context.TeamPlayers.FindAsync(id);
            if (teamPlayers == null)
            {
                return NotFound();
            }

            _context.TeamPlayers.Remove(teamPlayers);
            await _context.SaveChangesAsync();

            return teamPlayers;
        }

        private bool TeamPlayersExists(int id)
        {
            return _context.TeamPlayers.Any(e => e.TeamID == id);
        }
    }
}
