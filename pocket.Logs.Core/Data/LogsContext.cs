using Microsoft.EntityFrameworkCore;

namespace pocket.Logs.Core.Data
{
    public class LogsContext : DbContext
    {
        public LogsContext(DbContextOptions<LogsContext> options) : base(options)
        {
        }

        public DbSet<Player> Players => Set<Player>();

        public DbSet<PlayerRating> PlayerRatings => Set<PlayerRating>();

        public DbSet<PlayerRatingField> PlayerRatingFields => Set<PlayerRatingField>();

        public DbSet<RetrievedLog> RetrievedLogs => Set<RetrievedLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(e =>
            {
                e.HasKey(p => p.Id);
                e.HasIndex(p => p.SteamId).IsUnique();
            });

            modelBuilder.Entity<PlayerRating>(e =>
            {
                e.HasKey(p => p.Id);
                e.HasOne(p => p.Player).WithMany(p => p.Ratings).HasForeignKey(p => p.PlayerId);
            });

            modelBuilder.Entity<PlayerRatingField>(e =>
            {
                e.HasKey(p => p.Id);
                e.HasIndex(p => new { p.PlayerRatingId, p.Key }).IsUnique();
                e.HasOne(p => p.Player).WithMany().HasForeignKey(p => p.PlayerId);
                e.HasOne(p => p.Rating).WithMany(p => p.Fields).HasForeignKey(p => p.PlayerRatingId);
            });

            modelBuilder.Entity<RetrievedLog>(e =>
            {
                e.HasKey(p => p.Id);
                e.HasIndex(p => p.LogId).IsUnique();
            });
        }
    }
}