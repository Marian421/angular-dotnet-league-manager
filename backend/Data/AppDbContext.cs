using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor that passes options to the base DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define a DbSet for each table you want to create
        public DbSet<User> Users { get; set; }  // This will map to a "Users" table
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Match -> HomeTeam (1..many)
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Match -> AwayTeam (1..many)
            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Team -> Coach (1..1)
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Coach)
                .WithOne(u => u.TeamCoached)
                .HasForeignKey<Team>(t => t.CoachId);

            // Player -> User (1..1)
            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithOne(u => u.PlayerProfile)
                .HasForeignKey<Player>(p => p.UserId);

            // Player -> Team (many players -> one team)
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);
        }
    }
}