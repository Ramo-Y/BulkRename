namespace BulkRename.Helpers
{
    using BulkRename.Constants;

    internal static class FolderHelper
    {
        internal static string GetRootFolder()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var rootFolder = Path.Combine(currentDirectory, EnvironmentConstants.FILES_ROOT_PATH_FOLDER_NAME);
            return rootFolder;
        }

        internal static string GetHistoryFileName()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var historyFolder = Path.Combine(currentDirectory, EnvironmentConstants.FILES_HISTORY_PATH_FOLDER_NAME);
            if (!Directory.Exists(historyFolder))
            {
                Directory.CreateDirectory(historyFolder);
            }

            var historyFileName = Path.Combine(historyFolder, EnvironmentConstants.FILES_HISTORY_FILE_NAME);

            return historyFileName;
        }
    }
}