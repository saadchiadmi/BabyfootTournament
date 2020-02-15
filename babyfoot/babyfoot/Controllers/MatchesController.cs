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
using System.Transactions;
using babyfoot.Rules;
using babyfoot.RequestManagers;

namespace babyfoot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly BabyfootDbContext context;
        private readonly ViewMaker view_maker;
        private readonly MatchManager match_manager;

        public MatchesController(BabyfootDbContext context)
        {
            this.context = context;
            this.view_maker = new ViewMaker(context);
            this.match_manager = new MatchManager(context);
        }

        // GET: api/Matches/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<MatchView>> GetMatch(String token)
        {
            if (token == null)
                return BadRequest();

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

            var view = view_maker.MatchView(match);

            return view;
        }

        // DELETE: api/Matches/{token}/{pseudo}/Goals
        [HttpDelete("{token}/{pseudo}/Goals")]
        public IActionResult RemoveGoal(String token, String pseudo)
        {
            if (token == null)
                return BadRequest();

            if (pseudo == null)
                return BadRequest();

            var match = context.Matches.Where(t => t.Token.Equals(token))
            .Include(t => t.Tournament)
            .Include(t => t.GoalsOfMatch)
            .Include(t => t.TeamsOfMatch)
                .ThenInclude(t => t.Team)
                .ThenInclude(t => t.PlayersOfTeam)
                .ThenInclude(t => t.Player)
                .FirstOrDefault();

            if (match == null)
                return NotFound();

            try
            {
                using (var scope = new TransactionScope())
                {
                    match_manager.RemoveGoal(pseudo, match);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.MatchView(match);

            var action = RedirectToAction("RemoveGoal", null, new_view);

            return action;
        }

        // POST: api/Matches/{token}/{pseudo}/Goals
        [HttpPost("{token}/{pseudo}/Goals")]
        public IActionResult AddGoal(String token, String pseudo)
        {
            if (token == null)
                return BadRequest();

            if (pseudo == null)
                return BadRequest();

            var match = context.Matches.Where(t => t.Token.Equals(token))
            .Include(t => t.Tournament)
            .Include(t => t.GoalsOfMatch)
            .Include(t => t.TeamsOfMatch)
                .ThenInclude(t => t.Team)
                .ThenInclude(t => t.PlayersOfTeam)
                .ThenInclude(t => t.Player)
                .FirstOrDefault();

            if (match == null)
                return NotFound();

            if (match.State != MatchState.InProgress)
                return UnprocessableEntity();

            try
            {
                using (var scope = new TransactionScope())
                {
                    match_manager.AddGoal(pseudo, match);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.MatchView(match);

            var action = RedirectToAction("AddGoal", null, new_view);

            return action;
        }

        // PUT: api/Matches/{token}/End
        [HttpPut("{token}/End")]
        public IActionResult End(String token)
        {
            if (token == null)
                return BadRequest();

            var match = context.Matches.Where(t => t.Token.Equals(token))
                .Include(t => t.Tournament)
                .Include(t => t.GoalsOfMatch)
                    .ThenInclude(t => t.Player)
                .Include(t => t.TeamsOfMatch)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(t => t.PlayersOfTeam)
                    .ThenInclude(t => t.Player)
                    .FirstOrDefault();

            if (match == null)
                return NotFound();

            try
            {
                using (var scope = new TransactionScope())
                {
                    match_manager.End(match);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.MatchView(match);

            var action = RedirectToAction("End", null, new_view);

            return action;
        }



        // PUT: api/Matches/{token}/Start
        [HttpPut("{token}/Start")]
        public IActionResult Start(String token)
        {
            if (token == null)
                return BadRequest();

            var match = context.Matches.Where(t => t.Token.Equals(token))
                    .FirstOrDefault();

            if (match == null)
                return NotFound();

            try
            {
                using (var scope = new TransactionScope())
                {
                    match_manager.Start(match);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.MatchView(match);

            var action = RedirectToAction("Start", null, new_view);

            return action;
        }

        // PUT: api/Matches/{token}/Restart
        [HttpPut("{token}/Restart")]
        public IActionResult Restart(String token)
        {
            if (token == null)
                return BadRequest();

            var match = context.Matches.Where(t => t.Token.Equals(token))
                    .FirstOrDefault();

            if (match == null)
                return NotFound();

            try
            {
                using (var scope = new TransactionScope())
                {
                    match_manager.Restart(match);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.MatchView(match);

            var action = RedirectToAction("Restart", null, new_view);

            return action;
        }

        // PUT: api/Matches/{token}/Pause
        [HttpPut("{token}/Pause")]
        public IActionResult Pause(String token)
        {
            if (token == null)
                return BadRequest();

            var match = context.Matches.Where(t => t.Token.Equals(token))
                    .FirstOrDefault();

            if (match == null)
                return NotFound();

            try
            {
                using (var scope = new TransactionScope())
                {
                    match_manager.Pause(match);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.MatchView(match);

            var action = RedirectToAction("Pause", null, new_view);

            return action;
        }

    }
}
