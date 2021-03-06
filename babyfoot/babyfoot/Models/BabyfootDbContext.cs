﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Models
{
    public class BabyfootDbContext : DbContext
    {
        public BabyfootDbContext(DbContextOptions options) : base(options)
        {

        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Player>().Property(t => t.PlayerId).ValueGeneratedOnAdd();
            model.Entity<Team>().Property(t => t.TeamId).ValueGeneratedOnAdd();
            model.Entity<Match>().Property(t => t.MatchId).ValueGeneratedOnAdd();
            model.Entity<Tournament>().Property(t => t.TournamentId).ValueGeneratedOnAdd();

            model.Entity<Player>()      .ToTable("Players");
            model.Entity<Team>()        .ToTable("Teams");
            model.Entity<Match>()       .ToTable("Matches");
            model.Entity<Tournament>()  .ToTable("Tournaments");

            model.Entity<PlayerGoal>()  .ToTable("PlayerGoals");
            model.Entity<PlayerTeam>()  .ToTable("PlayerTeam");
            model.Entity<MatchTeam>()   .ToTable("MatchTeam");

            model.Entity<Player>()      .HasKey(t => t.PlayerId)        .IsClustered();
            model.Entity<Team>()        .HasKey(t => t.TeamId)          .IsClustered();
            model.Entity<Match>()       .HasKey(t => t.MatchId)         .IsClustered();
            model.Entity<Tournament>()  .HasKey(t => t.TournamentId)    .IsClustered();

            model.Entity<PlayerTeam>()  .HasKey(t => new { t.TeamId, t.PlayerId })   .IsClustered();
            model.Entity<PlayerGoal>()  .HasKey(t => new { t.MatchId, t.PlayerId })  .IsClustered();
            model.Entity<MatchTeam>()   .HasKey(t => new { t.MatchId, t.TeamId })    .IsClustered();


            model.Entity<PlayerTeam>()  .HasOne(t => t.Team)    .WithMany(t => t.PlayersOfTeam) .HasForeignKey(t => t.TeamId)   .IsRequired();
            model.Entity<PlayerTeam>()  .HasOne(t => t.Player)  .WithMany(t => t.TeamsOfPlayer) .HasForeignKey(t => t.PlayerId) .IsRequired();
            model.Entity<PlayerGoal>()  .HasOne(t => t.Player)  .WithMany(t => t.GoalsOfPlayer) .HasForeignKey(t => t.PlayerId) .IsRequired();
            model.Entity<PlayerGoal>()  .HasOne(t => t.Match)   .WithMany(t => t.GoalsOfMatch)  .HasForeignKey(t => t.MatchId)  .IsRequired();
            model.Entity<MatchTeam>()   .HasOne(t => t.Team)    .WithMany(t => t.MatchesOfTeam) .HasForeignKey(t => t.TeamId)   .IsRequired();
            model.Entity<MatchTeam>()   .HasOne(t => t.Match)   .WithMany(t => t.TeamsOfMatch)  .HasForeignKey(t => t.MatchId)  .IsRequired();

            model.Entity<Match>()       .HasOne(t => t.Tournament).WithMany(t => t.Matches)     .HasForeignKey(t => t.TournamentId).IsRequired();

            base.OnModelCreating(model);

        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }

        public DbSet<PlayerTeam> TeamPlayers { get; set; }
        public DbSet<PlayerGoal> PlayerGoals { get; set; }
        public DbSet<MatchTeam> MatchTeams { get; set; }
    }
}
