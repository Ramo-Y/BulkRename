namespace BulkRename.IntegrationTests.Services
{
    using BulkRename.IntegrationTests.Constants;
    using BulkRename.IntegrationTests.Helpers;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Text;

    [TestFixture]
    public class FileServiceTests
    {
        private const string WritableDirectoryPermissions = "777";
        private const string WritableFilePermissions = "666";

        private static void DeleteFilesRecursive(string mappedFilesFolderPath)
        {
            var directoryInfo = new DirectoryInfo(mappedFilesFolderPath);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);

            var allDirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            foreach (var sourceDirectory in allDirectories)
            {
                var destinationDirectory = sourceDirectory.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(destinationDirectory);
            }

            var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            foreach (var sourceFile in allFiles)
            {
                var destinationFile = sourceFile.Replace(sourcePath, targetPath);
                var destinationDirectory = Path.GetDirectoryName(destinationFile);

                if (!string.IsNullOrWhiteSpace(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                File.Copy(sourceFile, destinationFile, true);
            }

            EnsureWritablePermissions(targetPath);
        }

        private static void EnsureWritablePermissions(string targetPath)
        {
            if (!(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                  RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
            {
                return;
            }

            if (!Directory.Exists(targetPath))
            {
                return;
            }

            RunChmod(WritableDirectoryPermissions, targetPath);

            foreach (var directory in Directory.GetDirectories(targetPath, "*", SearchOption.AllDirectories))
            {
                RunChmod(WritableDirectoryPermissions, directory);
            }

            foreach (var file in Directory.GetFiles(targetPath, "*", SearchOption.AllDirectories))
            {
                RunChmod(WritableFilePermissions, file);
            }
        }

        private static void RunChmod(string mode, string path)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"{mode} \"{path}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var standardOutput = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"chmod failed for path '{path}' with mode '{mode}'. ExitCode: {process.ExitCode}. " +
                    $"StdOut: {standardOutput}. StdErr: {standardError}");
            }
        }

        private async Task WriteHighlightedOutput(string message)
        {
            await Console.Out.WriteLineAsync("################################################");
            await Console.Out.WriteLineAsync(message);
            await Console.Out.WriteLineAsync("################################################");
        }

        private static string ErrorMessage(IEnumerable<string> expectedItems, IEnumerable<string> actualItems)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("The files aren't renamed correctly.");
            stringBuilder.AppendLine("Expected files:");
            foreach (var item in expectedItems)
            {
                stringBuilder.AppendLine(item);
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Actual files:");
            foreach (var item in actualItems)
            {
                stringBuilder.AppendLine(item);
            }

            return stringBuilder.ToString();
        }

        private List<string> GetVideoFilesFromFolder(string path, IEnumerable<string> supportedFileEndings)
        {
            var filesFromFolder = new List<string>();
            filesFromFolder = supportedFileEndings.Aggregate(
                filesFromFolder,
                (current, s) => current.Union(new DirectoryInfo(path).GetFiles().Select(o => o.Name).Where(x => x.EndsWith(s))).OrderBy(f => f).ToList());

            return filesFromFolder;
        }

        [Test]
        public async Task CopyFilesToRenameToDockerMappedPort_SubmitRename_FilesAreRenamedCorrect()
        {
            // arrange
            const string SEASON_0 = "Season 0";
            const string THE_WALKING_DEAD = "The Walking Dead (2010)";
            const string DARK = "Dark (2017)";

            ConfigurationHelper.SetEnvironmentVariables();

            var currentDirectory = Directory.GetCurrentDirectory();
            await Console.Out.WriteLineAsync($"Current directory is {currentDirectory}");
            var testResourcesPath = Path.Combine(currentDirectory, TestConstants.TEST_RESOURCES);
            await Console.Out.WriteLineAsync($"Test resources directory is {testResourcesPath}");
            var mappedFilesFolderPath = ConfigurationHelper.GetContainerMappedFilesFolderPath();
            await Console.Out.WriteLineAsync($"Mapped container path is {mappedFilesFolderPath}");
            var supportedFileEndings = ConfigurationHelper.GetSupportedFileEndings();
            await Console.Out.WriteLineAsync($"Supported file endings are {string.Join(";", supportedFileEndings)}");

            DeleteFilesRecursive(mappedFilesFolderPath);
            CopyFilesRecursively(testResourcesPath, mappedFilesFolderPath);

            var darkExpectedNames = new List<string>
                                        {
                                            $"{DARK} - S01E01.mkv",
                                            $"{DARK} - S01E02.mkv",
                                            $"{DARK} - S01E03.mkv",
                                            $"{DARK} - S01E04.mkv",
                                            $"{DARK} - S01E05.mkv",
                                            $"{DARK} - S01E06.mkv",
                                            $"{DARK} - S01E07.mkv",
                                            $"{DARK} - S01E08.mkv",
                                            $"{DARK} - S01E09.mkv",
                                            $"{DARK} - S01E10.mkv"
                                        };

            var theWalkingDeadSeason01ExpectedNames = new List<string>
                                                          {
                                                              $"{THE_WALKING_DEAD} - S01E01.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E02.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E03.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E04.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E05.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E06.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E07.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E08.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E09.mkv",
                                                              $"{THE_WALKING_DEAD} - S01E10.mkv"
                                                          };

            var theWalkingDeadSeason02ExpectedNames = new List<string>
                                                          {
                                                              $"{THE_WALKING_DEAD} - S02E01.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E02.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E03.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E04.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E05.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E06.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E07.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E08.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E09.mkv",
                                                              $"{THE_WALKING_DEAD} - S02E10.mkv"
                                                          };

            // act
            var previewUri = RequestHelper.GetSeriesPreviewUri();
            await WriteHighlightedOutput($"PREVIEW URI IS: {previewUri}");
            var submitUri = RequestHelper.GetRenameSubmitUri();
            await WriteHighlightedOutput($"SUBMIT URI IS: {submitUri}");

            var client = new HttpClient();
            var previewResponseBody = await client.GetStringAsync(previewUri);
            await WriteHighlightedOutput($"Preview response Value is: {previewResponseBody}");

            var responseBody = await client.GetStringAsync(submitUri);
            await WriteHighlightedOutput($"Response Value is: {responseBody}");

            var darkPath = Path.Combine(mappedFilesFolderPath, DARK, $"{SEASON_0}1");
            var darkActualNames = GetVideoFilesFromFolder(darkPath, supportedFileEndings);
            var theWalkingDeadSeason01Path = Path.Combine(mappedFilesFolderPath, THE_WALKING_DEAD, $"{SEASON_0}1");
            var theWalkingDeadSeason01ActualNames = GetVideoFilesFromFolder(theWalkingDeadSeason01Path, supportedFileEndings);
            var theWalkingDeadSeason02Path = Path.Combine(mappedFilesFolderPath, THE_WALKING_DEAD, $"{SEASON_0}2");
            var theWalkingDeadSeason02ActualNames = GetVideoFilesFromFolder(theWalkingDeadSeason02Path, supportedFileEndings);

            using (Assert.EnterMultipleScope())
            {
                // assert
                Assert.That(darkActualNames, Is.EqualTo(darkExpectedNames), ErrorMessage(darkExpectedNames, darkActualNames));
                Assert.That(
                    theWalkingDeadSeason01ActualNames,
                    Is.EqualTo(theWalkingDeadSeason01ExpectedNames),
                    ErrorMessage(theWalkingDeadSeason01ExpectedNames, theWalkingDeadSeason01ActualNames));
                Assert.That(
                    theWalkingDeadSeason02ActualNames,
                    Is.EqualTo(theWalkingDeadSeason02ExpectedNames),
                    ErrorMessage(theWalkingDeadSeason02ExpectedNames, theWalkingDeadSeason02ActualNames));
            }
        }
    }
}