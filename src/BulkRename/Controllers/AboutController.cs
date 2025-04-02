namespace BulkRename.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using System.Reflection;

    public class AboutController : Controller
    {
        private const int START_INDEX = 0;
        private const int OFFSET = 1;

        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public AboutController(IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string GetCommitHash()
        {
            var informationalVersion = GetInformationalVersion();
            var index = informationalVersion.IndexOf('+');
            var toDeleteCount = index + OFFSET;
            var commitHash = informationalVersion.Remove(START_INDEX, toDeleteCount);
            return commitHash;
        }

        public string GetInformationalVersion()
        {
            var assembly = GetType().Assembly;
            var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var informationalVersion = informationalVersionAttribute!.InformationalVersion;
            return informationalVersion;
        }
    }
}