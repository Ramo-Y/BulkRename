namespace BulkRename.Controllers
{
    using BulkRename.Interfaces;
    using BulkRename.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    public class AboutController : Controller
    {
        private readonly IVersionService _versionService;

        public AboutController(IVersionService versionService)
        {
            _versionService = versionService;
        }

        public IActionResult Index()
        {
            var repositoryUrl = _versionService.GetRepositoryUrl();
            var commitHash = _versionService.GetCommitHash();

            var model = new AboutViewModel
            {
                AppVersion = _versionService.GetAppVersion(),
                Authors = _versionService.GetAuthors(),
                InformationalVersion = _versionService.GetInformationalVersion(),
                CommitHash = commitHash,
                BuildDate = _versionService.GetBuildDate(),
                RepositoryUrl = repositoryUrl,
                Copyright = _versionService.GetCopyright(),
                CommitUrl = $"{repositoryUrl.TrimEnd('/')}/commit/{commitHash}",
                SupportProjectUrl = _versionService.GetSupportProjectUrl(),
                IssuesUrl = $"{repositoryUrl.TrimEnd('/')}/issues",
            };

            return View(model);
        }
    }
}
