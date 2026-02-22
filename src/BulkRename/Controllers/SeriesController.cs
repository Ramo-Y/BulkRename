namespace BulkRename.Controllers
{
    using BulkRename.Interfaces;
    using BulkRename.Models;
    using BulkRename.Models.Entities;
    using Microsoft.AspNetCore.Mvc;

    public class SeriesController : Controller
    {
        private static readonly List<Episode> _allFileAndFolderItemsFromRootFolder = [];
        private static readonly Dictionary<string, List<Series>> _dictionary = [];

        private readonly ILogger<SeriesController> _logger;
        private readonly IFileService _fileService;

        public SeriesController(ILogger<SeriesController> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            _allFileAndFolderItemsFromRootFolder.Clear();
            _dictionary.Clear();

            _allFileAndFolderItemsFromRootFolder.AddRange(_fileService.GetAllFileAndFolderItemsFromRootFolder());

            var seasons = _allFileAndFolderItemsFromRootFolder.GroupBy(e => e.Season);
            foreach (var season in seasons)
            {
                var serieSerName = season.Key.Serie.SerName;
                var episodes = _allFileAndFolderItemsFromRootFolder.Where(e => e.Season.Equals(season.Key));
                var series = new List<Series>();
                foreach (var episode in episodes)
                {
                    series.Add(
                        new Series
                            {
                                OldName = episode.EpsEpisodeOriginalName,
                                NewName = episode.EpsEpisodeNewName,
                                FileSizeInMb = episode.EpsEpisodeFileSizeInMb
                            });
                }

                var seasonName = $"{serieSerName} - Season {season.Key.SsnNumberString}";
                _logger.LogInformation($"{seasonName} has {episodes.Count()} episodes to rename");
                _dictionary.Add(seasonName, series);
            }

            _logger.LogInformation($"{seasons.Count()} Seasons to rename", seasons);

            return View(_dictionary);
        }

        public async Task<IActionResult> RenameAsync()
        {
            await _fileService.RenameSelectedEpisodeItems(_allFileAndFolderItemsFromRootFolder);
            return View("Rename", _dictionary);
        }
    }
}