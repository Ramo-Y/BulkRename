namespace BulkRename.Interfaces
{
    using BulkRename.Models.Entities;

    public interface IFileService
    {
        IEnumerable<Episode> GetAllFileAndFolderItemsFromRootFolder();

        Task<IEnumerable<RenamingSessionToEpisode>> RenameSelectedEpisodeItems(IEnumerable<Episode> episodeItems);
    }
}