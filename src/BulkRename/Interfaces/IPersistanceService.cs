namespace BulkRename.Interfaces
{
    using BulkRename.Models.Entities;

    public interface IPersistanceService
    {
        Task PersistRenamingInformation(IEnumerable<RenamingSessionToEpisode> renamingSessionToEpisodes, CancellationToken cancellationToken = default);

        Task<IEnumerable<RenamingSessionToEpisode>> LoadRenamingHistory(CancellationToken cancellationToken = default);
    }
}