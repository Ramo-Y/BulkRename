namespace BulkRename.Controllers
{
    using BulkRename.Interfaces;
    using BulkRename.Models;
    using Microsoft.AspNetCore.Mvc;

    public class HistoryController : Controller
    {
        private readonly IPersistanceService _persistanceService;

        private static readonly Dictionary<string, List<Series>> _dictionary = [];

        public HistoryController(IPersistanceService persistanceService)
        {
            _persistanceService = persistanceService;
        }

        public IActionResult Index()
        {
            return View(_dictionary);
        }

        public async Task<IActionResult> LoadHistory()
        {
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

                var key = $"{renamingSessionToEpisode.RenamingSession.RenName}, Renamed on: {renamingSessionToEpisode.RenamingSession.RenExecutingDateTime:yyyy-MM-dd HH:mm:ss}";
                _dictionary.Add(key, series);
            }

            return RedirectToAction("Index");
        }
    }
}