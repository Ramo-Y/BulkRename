namespace BulkRename.Services
{
    using BulkRename.Interfaces;
    using BulkRename.Models.Entities;
    using Microsoft.EntityFrameworkCore;

    public class DatabasePersistanceService : IPersistanceService
    {
        private readonly BulkRenameContext _context;

        public DatabasePersistanceService(BulkRenameContext context)
        {
            _context = context;
        }

        public async Task PersistRenamingInformation(IEnumerable<RenamingSessionToEpisode> renamingSessionToEpisodes, CancellationToken cancellationToken = default)
        {
            if (renamingSessionToEpisodes.Any())
            {
                await _context.RenamingSessionToEpisodeItems.AddRangeAsync(renamingSessionToEpisodes, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await _context.DisposeAsync();
            }
        }

        public async Task<IEnumerable<RenamingSessionToEpisode>> LoadRenamingHistory(CancellationToken cancellationToken = default)
        {
            var renamingSessionToEpisodes = await _context.RenamingSessionToEpisodeItems
                .Include(x => x.Episode)
                .Include(x => x.RenamingSession)
                .Include(x => x.Episode.Season)
                .Include(x => x.Episode.Season.Serie)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            await _context.DisposeAsync();

            return renamingSessionToEpisodes;
        }
    }
}