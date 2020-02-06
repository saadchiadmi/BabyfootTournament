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

        // GET: api/Players/{pseudo}
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

        // POST: api/Players
        [HttpPost]
        public async Task<ActionResult<PlayerView>> PostPlayer(NewPlayerView view)
        {
            if (context.Players.Any(t => t.Pseudo.Equals(view.Pseudo)))
                return BadRequest();

            using (var transaction = context.Database.BeginTransaction())
            {
                var player = new Player
                {
                    Pseudo = view.Pseudo,
                    Score = view.Score,
                    Goals = 0,
                    Champions = 0
                };

                context.Add(player);

                await context.SaveChangesAsync();
                transaction.Commit();
            }

            var action = CreatedAtAction("PostPlayer", new { pseudo = view.Pseudo }, view);

            return action;
        }

        // POST: api/Players
        [HttpPost("many")]
        public async Task<ActionResult<PlayerView>> PostPlayers(List<NewPlayerView> view)
        {
            if (context.Players.Any(t => view.Select(t => t.Pseudo).Any(p => p.Equals(t.Pseudo))))
                return BadRequest();
            if (!(view.Select(t => t.Pseudo).Distinct().Count() == view.Count()))
                return BadRequest();


            using (var transaction = context.Database.BeginTransaction())
            {
                foreach (NewPlayerView pview in view)
                {
                    var player = new Player
                    {
                        Pseudo = pview.Pseudo,
                        Score = pview.Score,
                        Goals = 0,
                        Champions = 0
                    };

                    context.Add(player);
                }

                await context.SaveChangesAsync();

                transaction.Commit();
            }

            var action = CreatedAtAction("PostPlayers", null, view);

            return action;
        }

    }
}
