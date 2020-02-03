using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using babyfoot.Models;
using babyfoot.Views;

namespace babyfoot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly BabyfootDbContext context;
        private readonly BabyfootWebInterface webInterface;

        public PlayersController(BabyfootDbContext context)
        {
            this.context = context;
            this.webInterface = new BabyfootWebInterface(context);
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerView>>> GetPlayers()
        {
            var players = await context.Players
                .Select(t => webInterface.View(t))
                .ToListAsync();

            return players;
        }

        // GET: api/Players/pseudo
        [HttpGet("{pseudo}")]
        public async Task<ActionResult<PlayerView>> GetPlayer(String pseudo)
        {
            var player = await context.Players
                .FirstOrDefaultAsync(t => t.Pseudo.Equals(pseudo));

            if (player == null)
                return NotFound();

            var info = webInterface.View(player);

            return info;
        }

        // POST: api/Players/Add
        [HttpPost("Add")]
        public async Task<ActionResult<PlayerView>> PostPlayer(PlayerView view)
        {
            if (context.Players.Any(t => t.Pseudo.Equals(view.Pseudo)) || view.Goals != 0 || view.Champions != 0)
                return BadRequest();

            var player = new Player
            {
                Pseudo = view.Pseudo,
                Score = view.Score,
                Goals = view.Goals,
                Champions = view.Champions
            };

            context.Add(player);

            await context.SaveChangesAsync();

            var action = CreatedAtAction("PostPlayer", new { pseudo = view.Pseudo }, view);

            return action;
        }

    }
}
