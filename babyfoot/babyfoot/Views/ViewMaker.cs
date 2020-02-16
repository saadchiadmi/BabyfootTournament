using babyfoot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Views
{
    public class ViewMaker
    {
        private readonly BabyfootDbContext context;

        public ViewMaker(BabyfootDbContext context)
        {
            this.context = context;
        }

        // View = get view of an entity
        // Modify = get entity from db and modify + save it
        // Create = create a non existing entity and add + save it



        public PlayerView PlayerView(Player entity)
        {
            return new PlayerView()
            {
                Pseudo = entity.Pseudo,
                Score = entity.Score,
                Goals = entity.Goals,
                Champions = entity.Champions
            };
        }



        public MatchPlayerView MatchPlayerView(PlayerGoal entity)
        {
            return new MatchPlayerView()
            {
                Pseudo = entity.Player.Pseudo,
                Goals = entity.Goals
            };
        }

        public MatchTeamView MatchTeamView(MatchTeam entity)
        {
            var p1 = entity.Team.PlayersOfTeam.ElementAt(0).Player;
            var p2 = entity.Team.PlayersOfTeam.ElementAt(1).Player;

            var view = new MatchTeamView()
            {
                MatchPlayerView(entity.Match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p1.Pseudo))),
                MatchPlayerView(entity.Match.GoalsOfMatch.First(t => t.Player.Pseudo.Equals(p2.Pseudo)))
            };
            return view;
        }

        public TournamentTeamView TournamentTeamView(Team entity)
        {
            var p1 = entity.PlayersOfTeam.ElementAt(0).Player;
            var p2 = entity.PlayersOfTeam.ElementAt(1).Player;

            var view = new TournamentTeamView()
            {
                Points = entity.Points,
                Pseudos = new List<String> { p1.Pseudo, p2.Pseudo }
            };
            return view;
        }


        public MatchView MatchView(Match match)
        {
            var view = new MatchView()
            {
                Token = match.Token,
                Order = match.Order,
                StageStr = match.Stage.ToString(),
                StateStr = match.State.ToString(),
                StartDate = match.StartDate,
                RestartDate = match.RestartDate,
                ElapsedSeconds = match.ElapsedSeconds
            };

            view.Teams = new List<MatchTeamView>
            {
                MatchTeamView(match.TeamsOfMatch.ElementAt(0)),
                MatchTeamView(match.TeamsOfMatch.ElementAt(1))
            };
            return view;
        }



        public TournamentView TournamentView(Tournament entity)
        {
            var view = new TournamentView()
            {
                Token = entity.Token,
                CreateDate = entity.CreateDate,
                StateStr = entity.State.ToString(),
                Teams = new List<TournamentTeamView>(entity.Matches.SelectMany(t => t.TeamsOfMatch).DistinctBy(t => t.TeamId).Select(t => TournamentTeamView(t.Team))),
                Matches = new List<MatchView>(entity.Matches.Select(t => MatchView(t)))
            };
            return view;
        }


        public ServerStateView ServerStateView()
        {
            var view = new ServerStateView();

            view.Players = context.Players.Select(t => PlayerView(t)).ToList();

            view.Tournaments = context.Tournaments
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.TeamsOfMatch)
                            .ThenInclude(t => t.Team)
                                .ThenInclude(t => t.PlayersOfTeam)
                                    .ThenInclude(t => t.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(t => t.GoalsOfMatch)
                        .Select(t => TournamentView(t)).ToList();

            return view;
        }

    }
}
