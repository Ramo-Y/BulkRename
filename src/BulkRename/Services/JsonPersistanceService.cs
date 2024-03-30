namespace BulkRename.Services
{
    using System.Collections.Generic;
    using System.Text;

    using BulkRename.Helpers;
    using BulkRename.Interfaces;
    using BulkRename.Models.Entities;

    using Newtonsoft.Json;

    public class JsonPersistanceService : IPersistanceService
    {
        public async Task PersistRenamingInformation(IEnumerable<RenamingSessionToEpisode> renamingSessionToEpisodes, CancellationToken cancellationToken = default)
        {
            var serializeObject = string.Empty;
            var filePath = FolderHelper.GetHistoryFileName();

            var objectFromExistingJson = await GetHistoryFromFile(cancellationToken);
            objectFromExistingJson.AddRange(renamingSessionToEpisodes);

            await Task.Factory.StartNew(
                () =>
                    {
                        serializeObject = JsonConvert.SerializeObject(objectFromExistingJson, Formatting.Indented);
                    }, cancellationToken);

            var bytes = Encoding.UTF8.GetBytes(serializeObject);
            await using (var sourceStream = new FileStream(
                             filePath,
                             FileMode.OpenOrCreate,
                             FileAccess.Write,
                             FileShare.None,
                             4096,
                             true))
            {
                sourceStream.SetLength(0);
                await sourceStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
            }
        }

        public async Task<IEnumerable<RenamingSessionToEpisode>> LoadRenamingHistory(CancellationToken cancellationToken = default)
        {
            IEnumerable<RenamingSessionToEpisode> renamingSessionToEpisodes = await GetHistoryFromFile(cancellationToken);

            return await Task.FromResult(renamingSessionToEpisodes);
        }

        private static async Task<List<RenamingSessionToEpisode>> GetHistoryFromFile(CancellationToken cancellationToken)
        {
            List<RenamingSessionToEpisode>? renamingSessionToEpisodes = [];
            var filePath = FolderHelper.GetHistoryFileName();

            if (!File.Exists(filePath))
            {
                return renamingSessionToEpisodes;
            }
            
            var text = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken);

            await Task.Factory.StartNew(
                () =>
                {
                    renamingSessionToEpisodes = JsonConvert.DeserializeObject<List<RenamingSessionToEpisode>>(text);
                }, cancellationToken);

            return renamingSessionToEpisodes;
        }
    }
}