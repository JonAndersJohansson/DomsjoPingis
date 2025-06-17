using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<Set> Sets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>()
                .HasIndex(p => new { p.Name, p.BirthDate })
                .IsUnique();

            modelBuilder.Entity<Match>()
                .HasQueryFilter(m => m.IsActive);

            modelBuilder.Entity<Match>()
                .Property(m => m.BestOfSets)
                .HasConversion<string>();

            modelBuilder.Entity<Match>()
                .HasMany(m => m.Sets)
                .WithOne(s => s.Match)
                .HasForeignKey(s => s.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .Property(p => p.Gender)
                .HasConversion<string>();

            modelBuilder.Entity<Player>()
                .HasMany(p => p.MatchesAsPlayer1)
                .WithOne(m => m.Player1)
                .HasForeignKey(m => m.Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.MatchesAsPlayer2)
                .WithOne(m => m.Player2)
                .HasForeignKey(m => m.Player2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
