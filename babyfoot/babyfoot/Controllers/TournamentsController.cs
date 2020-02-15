using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class TournamentsController : ControllerBase
    {
        private readonly BabyfootDbContext context;
        private readonly ViewMaker view_maker;
        private readonly TournamentManager tournament_manager;
        private readonly MatchManager match_manager;
        private readonly Synchronizer synchronizer;

        public TournamentsController(BabyfootDbContext context)
        {
            this.context = context;
            this.view_maker = new ViewMaker(context);
            this.tournament_manager = new TournamentManager(context);
            this.match_manager = new MatchManager(context);
            this.synchronizer = new Synchronizer(context);
        }

        
        // GET: api/Tournaments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentView>>> GetTournaments()
        {
            var tournaments = await context.Tournaments
                .Include(t => t.Matches)
                    .ThenInclude(t => t.TeamsOfMatch)
                        .ThenInclude(t => t.Team)
                            .ThenInclude(t => t.PlayersOfTeam)
                                .ThenInclude(t => t.Player)
                .Include(t => t.Matches)
                    .ThenInclude(t => t.GoalsOfMatch)
                        .ToListAsync();

            var views = new List<TournamentView>(tournaments.Select(t => view_maker.TournamentView(t)));

            return views;
        }

        // GET: api/Tournaments/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<TournamentView>> GetTournament(String token)
        {
            var tournament = await context.Tournaments
                .Where(t => t.Token.Equals(token))
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                            .FirstOrDefaultAsync();

            if (tournament == null)
                return NotFound();

            var view = view_maker.TournamentView(tournament);

            return view;
        }

        // PUT: api/Tournaments/{token}/AddFinal
        [HttpPost("{token}/Final")]
        public IActionResult AddFinal(String token, MatchMakingView view)
        {
            var tournament = context.Tournaments
                .Where(t => t.Token.Equals(token))
                .FirstOrDefault();

            if (tournament == null)
                return NotFound();

            if (!(tournament.State == TournamentState.Semifinal))
                return UnprocessableEntity();

            try
            {
                using (var scope = new TransactionScope())
                {
                    tournament_manager.AddFinal(view, tournament);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.TournamentView(tournament);

            var action = RedirectToAction("AddFinal", null, new_view);

            return action;
        }

        // PUT: api/Tournaments/{token}/Semifinals
        [HttpPost("{token}/Semifinals")]
        public IActionResult AddSemifinals(String token, List<MatchMakingView> view)
        {
            var tournament = context.Tournaments
                .Where(t => t.Token.Equals(token))
                .FirstOrDefault();

            if (tournament == null)
                return NotFound();

            if (!(tournament.State == TournamentState.Pool))
                return UnprocessableEntity();

            try
            {
                using (var scope = new TransactionScope())
                {
                    tournament_manager.AddSemifinals(view, tournament);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.TournamentView(tournament);

            var action = RedirectToAction("AddSemifinals", null, new_view);

            return action;
        }

        // PUT: api/Tournaments/{token}/Score
        [HttpPut("{token}/Score")]
        public async Task<IActionResult> Score(String token)
        {
            var tournament = await context.Tournaments
                .Where(t => t.Token.Equals(token))
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                            .FirstOrDefaultAsync();

            if (tournament == null)
                return NotFound();

            if (!(tournament.State == TournamentState.Final))
                return UnprocessableEntity();

            try
            {
                using (var scope = new TransactionScope())
                {
                    tournament_manager.Score(tournament);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.TournamentView(tournament);

            var action = RedirectToAction("Score", null, new_view);

            return action;
        }



        // PUT: api/Tournaments/{token}/End
        [HttpPut("{token}/End")]
        public async Task<IActionResult> End(String token)
        {
            var tournament = await context.Tournaments
                .Where(t => t.Token.Equals(token))
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                            .FirstOrDefaultAsync();

            if (tournament == null)
                return NotFound();

            if (!(tournament.State == TournamentState.Ended))
                return UnprocessableEntity();

            try
            {
                using (var scope = new TransactionScope())
                {
                    tournament_manager.End(tournament);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.TournamentView(tournament);

            var action = RedirectToAction("End", null, new_view);

            return action;
        }



        // PUT: api/Tournaments/{token}/EndAndScore
        [HttpPut("{token}/EndAndScore")]
        public async Task<IActionResult> EndAndScore(String token)
        {
            var tournament = await context.Tournaments
                .Where(t => t.Token.Equals(token))
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                            .FirstOrDefaultAsync();

            if (tournament == null)
                return NotFound();

            if (!(tournament.State == TournamentState.Ended))
                return UnprocessableEntity();

            try
            {
                using (var scope = new TransactionScope())
                {
                    tournament_manager.EndAndScore(tournament);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.TournamentView(tournament);

            var action = RedirectToAction("EndAndScore", null, new_view);

            return action;
        }

        // POST: api/Tournaments
        [HttpPost]
        public IActionResult Create(List<MatchMakingView> view)
        {
            Tournament tournament;

            try
            {
                using (var scope = new TransactionScope())
                {
                    tournament = tournament_manager.Create(view);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var new_view = view_maker.TournamentView(tournament);

            var action = CreatedAtAction("Create", null, new_view);

            return action;
        }

        // PUT: api/Tournaments/Synchronize
        [HttpPut("Synchronize")]
        public IActionResult Synchronize(ClientStateView view)
        {
            List<TournamentTokenRenameView> renames;

            try
            {
                using (var scope = new TransactionScope())
                {
                    renames = synchronizer.Synchronize(view);

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }

            var vserver = view_maker.ServerStateView();

            var action = RedirectToAction("Synchronize", null, new { State = vserver, Renames = renames });

            return action;
        }

    }
}
