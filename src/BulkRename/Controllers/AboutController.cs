namespace BulkRename.Controllers
{
    using System.Reflection;
    using BulkRename.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class AboutController : Controller
    {
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IVersionService _versionService;

        public AboutController(
            IStringLocalizer<SharedResource> sharedLocalizer,
            IVersionService versionService
        )
        {
            _sharedLocalizer = sharedLocalizer;
            _versionService = versionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string GetCommitHash()
        {
            var commitHash = _versionService.GetCommitHash();
            return commitHash;
        }
    }
}
