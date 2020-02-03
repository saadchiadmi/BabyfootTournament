using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using babyfoot.Models;
using Newtonsoft.Json;
using babyfoot.Views;

namespace babyfoot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly BabyfootDbContext context;
        private readonly BabyfootWebInterface webInterface;

        public MatchesController(BabyfootDbContext context)
        {
            this.context = context;
            this.webInterface = new BabyfootWebInterface(context);
        }

        // GET: api/Matches/token
        [HttpGet("{token}")]
        public async Task<ActionResult<MatchView>> GetMatch(String token)
        {
            var match = await context.Matches
                .Where(t => t.Token.Equals(token))
                .Include(t => t.Tournament)
                .Include(t => t.GoalsOfMatch)
                .Include(t => t.TeamsOfMatch)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(t => t.PlayersOfTeam)
                    .ThenInclude(t => t.Player)
                .FirstOrDefaultAsync();

            if (match == null)
            {
                return NotFound();
            }

            var view = webInterface.View(match);

            return view;
        }

        // PUT: api/Matches/token
        [HttpPut("{token}")]
        public async Task<IActionResult> PutMatch(String token, MatchView view)
        {
            if (!token.Equals(view.Token))
                return BadRequest();

            var before = await context.Matches.Where(t => t.Token.Equals(token))
                .Include(t => t.Tournament)
                .Include(t => t.GoalsOfMatch)
                .Include(t => t.TeamsOfMatch)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(t => t.PlayersOfTeam)
                    .ThenInclude(t => t.Player)
                    .FirstOrDefaultAsync();

            if (before == null)
                return NotFound();



            // check new match state, optional
            if (!(before.Stage.Equals(view.Stage)))
                return BadRequest();
            if (!(before.State <= view.State))
                return BadRequest();
            if (!(before.StartDate.Equals(view.StartDate)))
                return BadRequest();
            if (!(before.ElapsedSeconds <= view.ElapsedSeconds))
                return BadRequest();



            var match = webInterface.EntityToSaveAsync(view).Result;

            context.Entry(match).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
