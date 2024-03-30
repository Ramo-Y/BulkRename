namespace BulkRename.IntegrationTests.Helpers
{
    internal static class RequestHelper
    {
        private static string GetHomeUri()
        {
            var homeUri = $"http://localhost:{ConfigurationHelper.GetContainerExternalPort()}";
            return homeUri;
        }

        internal static string GetSeriesPreviewUri()
        {
            var previewUri = $"{GetHomeUri()}/Series";
            return previewUri;
        }

        internal static string GetRenameSubmitUri()
        {
            var submitUri = $"{GetSeriesPreviewUri()}/Rename";
            return submitUri;
        }
    }
}