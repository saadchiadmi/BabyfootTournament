using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using babyfoot.Models;
using babyfoot.Views;
using System.Transactions;
using babyfoot.Rules;
using babyfoot.RequestManagers;

namespace babyfoot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly BabyfootDbContext context;
        private readonly ViewMaker view_maker;

        public PlayersController(BabyfootDbContext context)
        {
            this.context = context;
            this.view_maker = new ViewMaker(context);
        }

        // GET: api/Players
        [HttpGet]
        public async Task<ActionResult<List<PlayerView>>> GetAll()
        {
            var vplayers = await context.Players
                .Select(t => view_maker.PlayerView(t))
                .ToListAsync();

            return vplayers;
        }

        // GET: api/Players/{pseudo}
        [HttpGet("{pseudo}")]
        public async Task<ActionResult<PlayerView>> Get(String pseudo)
        {
            if (pseudo == null)
                return BadRequest();

            var player = await context.Players
                .FirstOrDefaultAsync(t => t.Pseudo.Equals(pseudo));

            if (player == null)
                return NotFound();

            var vplayer = view_maker.PlayerView(player);

            return vplayer;
        }

        // POST: api/Players
        [HttpPost]
        public async Task<ActionResult<PlayerView>> PostPlayer(NewPlayerView view)
        {
            if (context.Players.Any(t => t.Pseudo.Equals(view.Pseudo)))
                return BadRequest();

            using (var scope = new TransactionScope())
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
                scope.Complete();
            }

            var action = CreatedAtAction("PostPlayer", new { pseudo = view.Pseudo }, view);

            return action;
        }

        // POST: api/Players/Many
        [HttpPost("List")]
        public ActionResult<List<PlayerView>> CreateMany(List<String> pseudos)
        {
            if (pseudos == null)
                return BadRequest();

            if (context.Players.Any(t => pseudos.Any(p => p.Equals(t.Pseudo))))
                return UnprocessableEntity();

            if (!(pseudos.Distinct().Count() == pseudos.Count()))
                return UnprocessableEntity();

            var players = new List<Player>();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var manager = new PlayerManager(context);

                players = manager.AddMany(pseudos);

                scope.Complete();
            }

            var vplayers = players.Select(t => view_maker.PlayerView(t));

            var action = CreatedAtAction("PostPlayersList", pseudos, vplayers);

            return action;
        }

    }
}
