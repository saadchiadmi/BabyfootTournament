using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using babyfoot;
using babyfoot.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace babyfoot.Models
{
    public class ConnectionDBClass:DbContext
    {
        public ConnectionDBClass(DbContextOptions<ConnectionDBClass> options): base(options)
        {
        }

        public DbSet<Players> Players { get; set; }
        public DbSet<Tournament> Tournament { get; set; }
        public DbSet<Match> Match { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchTeams>()
                .HasKey(c => new { c.MatchID, c.TeamID });
            modelBuilder.Entity<TeamPlayers>()
                .HasKey(c => new { c.TeamID, c.PlayerID });
            modelBuilder.Entity<Match>()
                .HasOne(p => p.Tournament)
                .WithMany(b => b.Matches)
                .IsRequired();
            modelBuilder.Entity<MatchTeams>()
                .HasOne(p => p.Match)
                .WithMany(b => b.MatchTeams)
                .HasForeignKey(p => p.MatchID)
                .IsRequired();
            modelBuilder.Entity<MatchTeams>()
                .HasOne(p => p.Team)
                .WithMany(b => b.MatchTeams)
                .HasForeignKey(p => p.TeamID)
                .IsRequired();
            /*modelBuilder.Entity<Tournament>().HasMany(x => x.Matches);
            modelBuilder.Entity<Match>().HasMany(e => e.MatchTeams);
            modelBuilder.Entity<Match>().HasMany(e => e.MatchGoals);
            modelBuilder.Entity<Players>().HasMany(e => e.MatchGoals);
            modelBuilder.Entity<Teams>().HasMany(e => e.MatchTeams);
            modelBuilder.Entity<Players>().HasMany(e => e.TeamPlayers);
            modelBuilder.Entity<Teams>().HasMany(e => e.TeamPlayers);*/
        }

        public DbSet<babyfoot.Models.MatchTeams> MatchTeams { get; set; }

        public DbSet<babyfoot.Models.MatchGoals> MatchGoals { get; set; }

        public DbSet<babyfoot.Models.Teams> Teams { get; set; }

        public DbSet<babyfoot.Models.TeamPlayers> TeamPlayers { get; set; }
    }
}
