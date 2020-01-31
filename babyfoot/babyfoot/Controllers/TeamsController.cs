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
    public class TeamsController : ControllerBase
    {
        private readonly BabyfootDbContext context;

        public TeamsController(BabyfootDbContext context)
        {
            this.context = context;
        }

        // GET: api/Team
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await context.Teams.ToListAsync();
        }

        // GET: api/Team/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeams(int id)
        {
            var teams = await context.Teams.Include(t => t.PlayersOfTeam)
                                            .ThenInclude(tp => tp.Player)
                                            .ThenInclude(p => p.GoalsOfPlayer)
                .FirstOrDefaultAsync(i => i.TeamId == id);

            if (teams == null)
            {
                return NotFound();
            }

            return teams;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeams(int id, Team teams)
        {
            if (id != teams.TeamId)
            {
                return BadRequest();
            }

            context.Entry(teams).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamsExists(id))
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

        // POST: api/Teams
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async /*Task<ActionResult<Teams>>*/Task PostTeams([FromBody] Team teams)
        {
            /*teams.TeamPlayers.Add(new TeamPlayers { TeamID = teams.ID, PlayerID = teams._player1.ID });
            teams.TeamPlayers.Add(new TeamPlayers { TeamID = teams.ID, PlayerID = teams._player2.ID });*/
            context.Teams.Add(teams);
            await context.SaveChangesAsync();

            //return CreatedAtAction("GetTeams", new { id = teams.ID }, teams);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Team>> DeleteTeams(int id)
        {
            var teams = await context.Teams.FindAsync(id);
            if (teams == null)
            {
                return NotFound();
            }

            context.Teams.Remove(teams);
            await context.SaveChangesAsync();

            return teams;
        }

        private bool TeamsExists(int id)
        {
            return context.Teams.Any(e => e.TeamId == id);
        }
    }
}
