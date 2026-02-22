namespace BulkRename.Services
{
    using BulkRename.Constants;
    using BulkRename.Helpers;
    using BulkRename.Interfaces;
    using BulkRename.Models.Entities;

    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<FileService> _logger;

        private readonly IPersistanceService _persistanceService;

        private readonly string _rootFolder;

        public FileService(IConfiguration configuration, ILogger<FileService> logger, IPersistanceService persistanceService)
        {
            _configuration = configuration;
            _logger = logger;
            _persistanceService = persistanceService;
            _rootFolder = FolderHelper.GetRootFolder();
        }

        public IEnumerable<Episode> GetAllFileAndFolderItemsFromRootFolder()
        {
            var serieFolders = Directory.GetDirectories(_rootFolder).AsEnumerable();
            _logger.LogInformation($"Root folder is '{_rootFolder}'", _rootFolder);

            var folderToIgnoreConfig = _configuration[ConfigurationNameConstants.FOLDERS_TO_IGNORE];
            if (!string.IsNullOrEmpty(folderToIgnoreConfig))
            {
                var foldersToIgnore = folderToIgnoreConfig.Split(';');
                serieFolders = foldersToIgnore.Aggregate(serieFolders, (current, s) => current.Where(x => !x.Contains(s)));
            }

            var serieItems = GetSerieItems(serieFolders);
            var seasonItems = GetSeasonItems(serieItems);
            var episodeItems = GetEpisodeItems(seasonItems);
            return episodeItems;
        }

        public async Task<IEnumerable<RenamingSessionToEpisode>> RenameSelectedEpisodeItems(IEnumerable<Episode> episodeItems)
        {
            var renamingSessionToEpisodes = new List<RenamingSessionToEpisode>();

            await Task.Run(
                () =>
                    {
                        var seasonItems = episodeItems.GroupBy(x => x.Season).Select(x => x.Key);
                        var renamingSessionItems = seasonItems.Select(GetSessionName).Select(
                            sessionName => new RenamingSession
                            {
                                RenamingSessionID = Guid.NewGuid(),
                                RenExecutingDateTime = DateTime.Now,
                                RenName = sessionName,
                                RenRenamingSessionIsOk = true
                            }).ToList();

                        foreach (var episode in episodeItems)
                        {
                            if (episode.EpsEpisodeOriginalName != episode.EpsEpisodeNewName)
                            {
                                var currentSession = renamingSessionItems.First(r => r.RenName == GetSessionName(episode.Season));
                                var sessionToEpisode = new RenamingSessionToEpisode
                                {
                                    RenamingSessionToEpisodeID = Guid.NewGuid(),
                                    RenamingSession = currentSession,
                                    RseRenamingSessionID_FK = currentSession.RenamingSessionID,
                                    Episode = episode,
                                    RseEpisodeID_FK = episode.EpisodeID
                                };
                                renamingSessionToEpisodes.Add(sessionToEpisode);
                                var episodeFileInfo = GetEpisodeFileInfo(episode);
                                var newPath = Path.Combine(GetSeasonPath(episode), episode.EpsEpisodeNewName);
                                episodeFileInfo.MoveTo(newPath);
                            }
                        }
                    });

            await _persistanceService.PersistRenamingInformation(renamingSessionToEpisodes);

            return renamingSessionToEpisodes;
        }

        private string GetSessionName(Season season)
        {
            var sessionName = $"{season.Serie.SerName} - Season {season.SsnNumberString}";
            return sessionName;
        }

        private string GetSeasonPath(Episode episode)
        {
            var serieName = episode.Season.Serie.SerName;
            var seasonNumberString = episode.Season.SsnNumberString;
            var seasonPath = Path.Combine(_rootFolder, serieName, $"Season {seasonNumberString}");

            return seasonPath;
        }

        private FileInfo GetEpisodeFileInfo(Episode episode)
        {
            var seasonPath = GetSeasonPath(episode);
            var episodeEpsEpisodeOriginalName = episode.EpsEpisodeOriginalName;
            var episodePath = Path.Combine(seasonPath, episodeEpsEpisodeOriginalName);
            var fileInfo = new FileInfo(episodePath);

            return fileInfo;
        }

        private IEnumerable<Serie> GetSerieItems(IEnumerable<string> serieFolders)
        {
            var serieItems = serieFolders.Select(
                folder => new Serie
                {
                    SerieID = Guid.NewGuid(),
                    SerName = new DirectoryInfo(folder).Name
                }).ToList();

            _logger.LogInformation("Found the following series:");

            foreach (var item in serieItems)
            {
                _logger.LogInformation($"{item.SerName}", item);
            }

            return serieItems;
        }

        private IEnumerable<Season> GetSeasonItems(IEnumerable<Serie> serieItems)
        {
            var seasonItems = new List<Season>();

            foreach (var serieItem in serieItems)
            {
                var seasonFolders = Directory.GetDirectories(Path.Combine(_rootFolder, serieItem.SerName));
                seasonItems.AddRange(
                    seasonFolders.Select(
                        seasonFolder => new Season
                        {
                            SeasonID = Guid.NewGuid(),
                            Serie = serieItem,
                            SsnSerieID_FK = serieItem.SerieID,
                            SsnNumberString = seasonFolder.Substring(seasonFolder.Length - 2)
                        }));
            }

            return seasonItems;
        }

        private IEnumerable<Episode> GetEpisodeItems(IEnumerable<Season> seasonItems)
        {
            var episodeItems = new List<Episode>();

            foreach (var seasonItem in seasonItems)
            {
                var episodeStartNumber = RenamingConstants.DEFAULT_START_NUMBER;
                var seasonPath = Path.Combine(_rootFolder, seasonItem.Serie.SerName, $"Season {seasonItem.SsnNumberString}");

                var episodesPathItems = new List<string>();

                var supportedFileEndingsConfig = _configuration[ConfigurationNameConstants.SUPPORTED_FILE_ENDINGS];
                if (!string.IsNullOrEmpty(supportedFileEndingsConfig))
                {
                    var endings = supportedFileEndingsConfig.Split(';');
                    episodesPathItems = endings.Aggregate(
                        episodesPathItems,
                        (current, s) => current.Union(Directory.GetFileSystemEntries(seasonPath).Where(x => x.EndsWith(s))).OrderBy(x => x).ToList());
                }
                else
                {
                    episodesPathItems = Directory.GetFileSystemEntries(seasonPath).OrderBy(x => x).ToList();
                }

                foreach (var episodesPathItem in episodesPathItems)
                {
                    var fileInfo = new FileInfo(episodesPathItem);
                    var newName = seasonItem.Serie.SerName + RenamingConstants.NAME_FIRST_PRAEFIX + seasonItem.SsnNumberString + RenamingConstants.NAME_SECOND_PRAEFIX
                                  + $"{episodeStartNumber:D2}" + fileInfo.Extension;
                    var fileSizeInMB = fileInfo.Length / (1024.0 * 1024.0);
                    var episode = new Episode
                    {
                        EpisodeID = Guid.NewGuid(),
                        EpsSeasonID_FK = seasonItem.SeasonID,
                        Season = seasonItem,
                        EpsNumberString = $"{episodeStartNumber:D2}",
                        EpsEpisodeOriginalName = fileInfo.Name,
                        EpsEpisodeNewName = newName,
                        EpsEpisodeFileSizeInMb = fileSizeInMB,
                        EpsFileExtension = fileInfo.Extension,
                        EpsLastWriteTimeTimeUtc = fileInfo.LastWriteTimeUtc,
                    };

                    episodeItems.Add(episode);
                    episodeStartNumber++;
                }
            }

            return episodeItems;
        }
    }
}