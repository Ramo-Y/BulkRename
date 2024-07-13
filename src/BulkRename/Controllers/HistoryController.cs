namespace BulkRename.Controllers
{
    using BulkRename.Interfaces;
    using BulkRename.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class HistoryController : Controller
    {
        private readonly IPersistanceService _persistanceService;
        
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        private static readonly Dictionary<string, List<Series>> _dictionary = [];

        public HistoryController(IPersistanceService persistanceService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _persistanceService = persistanceService;
            _sharedLocalizer = sharedLocalizer;
        }

        public IActionResult Index()
        {
            return View(_dictionary);
        }

        public async Task<IActionResult> LoadHistory()
        {
            var renamedOn = _sharedLocalizer["RenamedOn"];
            _dictionary.Clear();

            var renamingSessionToEpisodes = await _persistanceService.LoadRenamingHistory();

            var seasons = renamingSessionToEpisodes.OrderByDescending(e => e.RenamingSession.RenExecutingDateTime)
                .GroupBy(e => e.Episode.Season.SeasonID);
            foreach (var season in seasons)
            {
                var renamingSessionToEpisode = renamingSessionToEpisodes.Where(x => x.Episode.EpsSeasonID_FK == season.Key)
                    .First();

                var episodes = renamingSessionToEpisodes.Where(e => e.Episode.Season.SeasonID == season.Key)
                    .OrderBy(x => x.Episode.EpsNumberString)
                    .Select(x => x.Episode);
                var series = new List<Series>();
                foreach (var episode in episodes)
                {
                    series.Add(
                        new Series
                        {
                            OldName = episode.EpsEpisodeOriginalName,
                            NewName = episode.EpsEpisodeNewName
                        });
                }

                var key = $"{renamingSessionToEpisode.RenamingSession.RenName}, {renamedOn}: {renamingSessionToEpisode.RenamingSession.RenExecutingDateTime:yyyy-MM-dd HH:mm:ss}";
                _dictionary.Add(key, series);
            }

            return RedirectToAction("Index");
        }
    }
}