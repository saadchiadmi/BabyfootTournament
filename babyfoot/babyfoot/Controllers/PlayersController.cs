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
        private readonly BabyfootDbContext context;

        public PlayersController(BabyfootDbContext context)
        {
            this.context = context;
        }

        // GET: api/players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            var players = await context.Players.ToListAsync();

            return players;
        }

        // POST: api/players
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayers(Player player)
        {
            if (context.Players.Any(t => t.Pseudo == player.Pseudo))
                return base.BadRequest();

            context.Players.Add(player);
            await context.SaveChangesAsync();

            var task = CreatedAtAction("GetPlayers", new { id = player.PlayerId }, player);

            return task;
        }

    }
}
