
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamApplication> TeamApplications { get; set; }
        public DbSet<Championship> Championships { get; set; }
        public DbSet<ChampionshipTeam> ChampionshipTeams { get; set; }
        public DbSet<ChampionshipApplication> ChampionshipApplications { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------------------------
            // Match -> Teams (home/away)
            // --------------------------


            modelBuilder.Entity<Match>()
              .HasOne(m => m.TeamA)
              .WithMany(t => t.MatchesAsTeamA)
              .HasForeignKey(m => m.TeamAId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
              .HasOne(m => m.TeamB)
              .WithMany(t => t.MatchesAsTeamB)
              .HasForeignKey(m => m.TeamBId)
              .OnDelete(DeleteBehavior.Restrict);

            // Match -> Championship (optional)
            modelBuilder.Entity<Match>()
              .HasOne(m => m.Championship)
              .WithMany(c => c.Matches)
              .HasForeignKey(m => m.ChampionshipId)
              .OnDelete(DeleteBehavior.SetNull);

            // --------------------------
            // Team -> Manager
            // --------------------------
            modelBuilder.Entity<Team>()
              .HasOne(t => t.Owner)
              .WithMany(u => u.ManagedTeams)
              .HasForeignKey(t => t.OwnerId)
              .OnDelete(DeleteBehavior.Restrict);

            // --------------------------
            // TeamMember -> Team & User
            // --------------------------
            modelBuilder.Entity<TeamMember>()
              .HasOne(tm => tm.Team)
              .WithMany(t => t.Members)
              .HasForeignKey(tm => tm.TeamId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
              .HasOne(tm => tm.User)
              .WithMany(u => u.TeamMemberships)
              .HasForeignKey(tm => tm.UserId)
              .OnDelete(DeleteBehavior.SetNull);

            // --------------------------
            // TeamApplication -> Team & User
            // --------------------------
            modelBuilder.Entity<TeamApplication>()
              .HasOne(ta => ta.Team)
              .WithMany()
              .HasForeignKey(ta => ta.TeamId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamApplication>()
              .HasOne(ta => ta.User)
              .WithMany()
              .HasForeignKey(ta => ta.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            // --------------------------
            // Championship -> Owner
            // --------------------------
            modelBuilder.Entity<Championship>()
              .HasOne(c => c.Owner)
              .WithMany(u => u.OwnedChampionships)
              .HasForeignKey(c => c.OwnerId)
              .OnDelete(DeleteBehavior.Restrict);

            // --------------------------
            // ChampionshipTeam (many-to-many join)
            // --------------------------
            modelBuilder.Entity<ChampionshipTeam>()
              .HasKey(ct => new { ct.ChampionshipId, ct.TeamId });

            modelBuilder.Entity<ChampionshipTeam>()
              .HasOne(ct => ct.Championship)
              .WithMany(c => c.Teams)
              .HasForeignKey(ct => ct.ChampionshipId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChampionshipTeam>()
              .HasOne(ct => ct.Team)
              .WithMany(t => t.Championships)
              .HasForeignKey(ct => ct.TeamId)
              .OnDelete(DeleteBehavior.Cascade);

            // --------------------------
            // ChampionshipApplication -> Championship & Team
            // --------------------------
            modelBuilder.Entity<ChampionshipApplication>()
              .HasOne(ca => ca.Championship)
              .WithMany(c => c.ChampionshipApplications)
              .HasForeignKey(ca => ca.ChampionshipId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChampionshipApplication>()
              .HasOne(ca => ca.Team)
              .WithMany()
              .HasForeignKey(ca => ca.TeamId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

