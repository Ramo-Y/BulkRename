namespace BulkRename.Interfaces
{
    public interface IVersionService
    {
        string GetAppVersion();

        string GetAuthors();

        string GetInformationalVersion();

        string GetCommitHash();

        DateTime GetBuildDate();

        string GetRepositoryUrl();

        string GetSupportProjectUrl();

        string GetCopyright();
    }
}
