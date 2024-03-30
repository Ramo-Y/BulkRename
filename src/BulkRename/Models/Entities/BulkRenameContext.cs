namespace BulkRename.Models.Entities
{
    using Microsoft.EntityFrameworkCore;

    public class BulkRenameContext : DbContext
    {
        public BulkRenameContext(DbContextOptions<BulkRenameContext> options)
            : base(options)
        {
        }

        public DbSet<Serie> SerieItems { get; set; }

        public DbSet<Season> SeasonItems { get; set; }

        public DbSet<Episode> EpisodeItems { get; set; }

        public DbSet<RenamingSession> RenamingSessionItems { get; set; }

        public DbSet<RenamingSessionToEpisode> RenamingSessionToEpisodeItems { get; set; }
    }
}