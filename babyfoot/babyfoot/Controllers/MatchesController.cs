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

        // GET: api/Matches/{token}
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
                return NotFound();

            var view = webInterface.View(match);

            return view;
        }

        // PUT: api/Matches/{token}
        [HttpPut("{token}")]
        public async Task<IActionResult> PutMatch(String token, MatchView view)
        {
            if (!token.Equals(view.Token))
                return BadRequest();

            var match = await context.Matches.Where(t => t.Token.Equals(token))
                .Include(t => t.Tournament)
                .Include(t => t.GoalsOfMatch)
                .Include(t => t.TeamsOfMatch)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(t => t.PlayersOfTeam)
                    .ThenInclude(t => t.Player)
                    .FirstOrDefaultAsync();

            if (match == null)
                return NotFound();

            if (!webInterface.CheckState(view, match))
                return BadRequest();

            await webInterface.ModifyAsync(view, match);

            return NoContent();
        }

        // PUT: api/Matches/{token}/points
        [HttpPut("{token}/points")]
        public async Task<IActionResult> PutEndedPoolMatch(String token, PointsView view)
        {
            var match = await context.Matches.Where(t => t.Token.Equals(token))
                .Include(t => t.Tournament)
                .Include(t => t.GoalsOfMatch)
                .Include(t => t.TeamsOfMatch)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(t => t.PlayersOfTeam)
                    .ThenInclude(t => t.Player)
                    .FirstOrDefaultAsync();

            if (match == null)
                return NotFound();

            if (!(match.State == MatchState.Ended))
                return BadRequest();

            match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(view[0].Pseudos[0]))).Team.Points = view[0].Points;
            match.TeamsOfMatch.First(t => t.Team.PlayersOfTeam.Any(t => t.Player.Pseudo.Equals(view[1].Pseudos[0]))).Team.Points = view[1].Points;

            context.Entry(match).State = EntityState.Modified;

            context.SaveChanges();

            return NoContent();
        }

    }
}
