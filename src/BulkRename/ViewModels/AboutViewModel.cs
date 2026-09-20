namespace BulkRename.ViewModels
{
    public class AboutViewModel
    {
        public string AppVersion { get; set; } = string.Empty;

        public string InformationalVersion { get; set; } = string.Empty;

        public string CommitHash { get; set; } = string.Empty;

        public DateTime BuildDate { get; set; }

        public string RepositoryUrl { get; set; } = string.Empty;

        public string Copyright { get; set; } = string.Empty;

        public string CommitUrl { get; set; } = string.Empty;

        public string SupportProjectUrl { get; set; } = string.Empty;
    }
}
