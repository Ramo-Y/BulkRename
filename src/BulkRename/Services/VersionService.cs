using System.Reflection;
using BulkRename.Constants;
using BulkRename.Interfaces;

namespace BulkRename.Services
{
    public class VersionService : IVersionService
    {
        private const int OFFSET = 1;

        public string GetAppVersion()
        {
            var assemblyVersion = GetType().Assembly.GetName().Version!;
            var version =
                $"V{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
            return version;
        }

        public string GetAuthors()
        {
            var metadataAttributes = GetMetadataAttributes();
            var attribute = metadataAttributes?.First(c =>
                c.Key.Equals(EnvironmentConstants.AUTHORS_ATTRIBUTE)
            );
            var url = attribute?.Value ?? string.Empty;
            return url;
        }

        public string GetCommitHash()
        {
            var informationalVersion = GetInformationalVersion();
            var index = informationalVersion.IndexOf('+');
            var toDeleteCount = index + OFFSET;
            var commitHash = informationalVersion[toDeleteCount..];
            return commitHash;
        }

        public string GetInformationalVersion()
        {
            var assembly = GetType().Assembly;
            var informationalVersionAttribute =
                assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var informationalVersion = informationalVersionAttribute!.InformationalVersion;
            return informationalVersion;
        }

        public DateTime GetBuildDate()
        {
            var metadataAttributes = GetMetadataAttributes();
            var attribute = metadataAttributes?.First(c => c.Key.Equals("BuildDate"));
            var dateString = attribute?.Value ?? string.Empty;
            DateTime.TryParseExact(
                dateString,
                "yyyyMMddHHmmss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var dateTime
            );
            return dateTime;
        }

        public string GetRepositoryUrl()
        {
            var metadataAttributes = GetMetadataAttributes();
            var attribute = metadataAttributes?.First(c =>
                c.Key.Equals(EnvironmentConstants.REPOSITORY_URL_ATTRIBUTE)
            );
            var url = attribute?.Value ?? string.Empty;
            return url;
        }

        public string GetSupportProjectUrl()
        {
            var metadataAttributes = GetMetadataAttributes();
            var attribute = metadataAttributes?.First(c =>
                c.Key.Equals(EnvironmentConstants.SUPPORT_PROJECT_URL_ATTRIBUTE)
            );
            var url = attribute?.Value ?? string.Empty;
            return url;
        }

        public string GetCopyright()
        {
            var assembly = GetType().Assembly;
            var attribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            var copyright = attribute?.Copyright ?? string.Empty;
            return copyright;
        }

        private IEnumerable<AssemblyMetadataAttribute> GetMetadataAttributes()
        {
            var assembly = GetType().Assembly;
            var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
            return metadataAttributes;
        }
    }
}
