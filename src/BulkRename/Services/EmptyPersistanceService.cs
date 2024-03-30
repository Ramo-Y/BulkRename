namespace BulkRename.Services
{
    using BulkRename.Interfaces;
    using BulkRename.Models.Entities;

    public class EmptyPersistanceService : IPersistanceService
    {
        public Task PersistRenamingInformation(IEnumerable<RenamingSessionToEpisode> renamingSessionToEpisodes, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<RenamingSessionToEpisode>> LoadRenamingHistory(CancellationToken cancellationToken = default)
        {
            var emptyList = new List<RenamingSessionToEpisode>().AsEnumerable();
            return Task.FromResult(emptyList);
        }
    }
}