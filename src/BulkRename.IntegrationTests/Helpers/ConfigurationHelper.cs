namespace BulkRename.IntegrationTests.Helpers
{
    using BulkRename.IntegrationTests.Constants;

    internal static class ConfigurationHelper
    {
        internal static void SetEnvironmentVariables()
        {
            var filePath = GetEnvironmentFilePath();
            var allLines = File.ReadAllLines(filePath);

            foreach (var line in allLines)
            {
                var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                {
                    continue;
                }

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }

        internal static string GetContainerExternalPort()
        {
            var port = Environment.GetEnvironmentVariable(ConfigurationConstants.BULK_RENAME_PORT)!;
            return port;
        }

        internal static string GetContainerMappedFilesFolderPath()
        {
            var folder = Environment.GetEnvironmentVariable(ConfigurationConstants.BULK_RENAME_FOLDER)!;
            return folder;
        }

        internal static string[] GetSupportedFileEndings()
        {
            var fileEndings = Environment.GetEnvironmentVariable(ConfigurationConstants.SUPPORTED_FILE_ENDINGS)!.Split(';');
            return fileEndings;
        }

        private static string GetEnvironmentFilePath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var solutionFolder = Directory.GetParent(currentDirectory)!.Parent?.Parent?.Parent?.ToString();
            var envFilePath = Path.Combine(solutionFolder!, ConfigurationConstants.ENV_FILE_NAME);
            return envFilePath;
        }
    }
}