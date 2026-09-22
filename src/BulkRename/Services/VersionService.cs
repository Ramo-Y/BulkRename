using System.Globalization;
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
            var attribute = GetMetadataAttribute(AttributeConstants.AUTHORS_ATTRIBUTE);
            var url = attribute?.Value ?? string.Empty;
            return url;
        }

        public string GetCommitHash()
        {
            var attribute = GetMetadataAttribute(AttributeConstants.GIT_COMMIT_ATTRIBUTE);
            var commitHash = attribute?.Value;

            if (string.IsNullOrWhiteSpace(commitHash))
            {
                commitHash = GetFallbackCommitHash();
            }

            return commitHash;
        }

        private string GetFallbackCommitHash()
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
            var attribute = GetMetadataAttribute(AttributeConstants.BUILD_DATE_ATTRIBUTE);
            var dateString = attribute?.Value ?? string.Empty;
            DateTime.TryParseExact(
                dateString,
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dateTime
            );
            return dateTime;
        }

        public string GetRepositoryUrl()
        {
            var attribute = GetMetadataAttribute(AttributeConstants.REPOSITORY_URL_ATTRIBUTE);
            var url = attribute?.Value ?? string.Empty;
            return url;
        }

        public string GetSupportProjectUrl()
        {
            var attribute = GetMetadataAttribute(AttributeConstants.SUPPORT_PROJECT_URL_ATTRIBUTE);
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

        private AssemblyMetadataAttribute? GetMetadataAttribute(string attributeName)
        {
            var assembly = GetType().Assembly;
            var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
            var attribute = metadataAttributes?.First(c =>
                c.Key.Equals(attributeName)
            );

            return attribute;

        }
    }
}
