namespace BulkRename.IntegrationTests.Resources
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    [TestFixture]
    internal class LanguageResourcesTests
    {
        private const string APP_RESOURCES = "SharedResource";

        private const string APP_RESOURCES_FILE_ENDING = "resx";

        private const string DEFAULT_LANGUAGE_CODE = "en";

        private const string DEFAULT_LANGUAGE_RESOURCE_FILENAME = "SharedResource.resx";

        private const int LANGUAGE_CODE_LENGTH = 2;

        private List<string> GetLanguageResourceFiles()
        {
            var files = Directory.GetFiles(@"../../../../BulkRename/Resources/", "*.resx").ToList();
            return files;
        }

        private List<LanguageResourceEntry> ReadLanguageResourceEntries(string filePath)
        {
            var list = new List<LanguageResourceEntry>();

            var document = new XmlDocument();
            document.Load(filePath);
            var node = document.GetElementsByTagName("data");
            foreach (XmlElement element in node)
            {
                var name = element.GetAttribute("name");
                var value = element.ChildNodes[1]!.InnerText;

                list.Add(new LanguageResourceEntry(name, value));
            }

            return list;
        }

        private Dictionary<string, List<LanguageResourceEntry>> GetLanguageDictionary()
        {
            var languageDictionary = new Dictionary<string, List<LanguageResourceEntry>>();
            var files = GetLanguageResourceFiles();

            var defaultLanguageFile = files.Single(f => Path.GetFileName(f).Equals(DEFAULT_LANGUAGE_RESOURCE_FILENAME));
            var defaultLanguageResourceEntries = ReadLanguageResourceEntries(defaultLanguageFile);
            languageDictionary.Add(DEFAULT_LANGUAGE_CODE, defaultLanguageResourceEntries);

            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                if (fileName == DEFAULT_LANGUAGE_RESOURCE_FILENAME)
                {
                    continue;
                }

                var parts = fileName.Split('.');
                var language = parts[1];
                var languageResourceEntries = ReadLanguageResourceEntries(filePath);

                languageDictionary.Add(language, languageResourceEntries);
            }

            return languageDictionary;
        }

        [Test]
        public void GetAllLanguageFiles_CheckNamingConvention_NamingIsCorrect()
        {
            var files = GetLanguageResourceFiles();

            var defaultFileErrorMessage = $"There should be a file called '{DEFAULT_LANGUAGE_RESOURCE_FILENAME}', it actually doesn't exist";
            Assert.That(files.Any(f => Path.GetFileName(f).Equals(DEFAULT_LANGUAGE_RESOURCE_FILENAME)), defaultFileErrorMessage);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (fileName == DEFAULT_LANGUAGE_RESOURCE_FILENAME)
                {
                    continue;
                }

                var parts = fileName.Split('.');
                var appResourcesPart = parts[0];
                var appResourcesPartErrorMessage = $"Language resource file name has to start with '{APP_RESOURCES}', actual one is '{appResourcesPart}'";
                Assert.That(appResourcesPart, Is.EqualTo(APP_RESOURCES), appResourcesPartErrorMessage);

                var languageCodePart = parts[1];
                var languageCodeErrorMessage = $"Language code should be {LANGUAGE_CODE_LENGTH} characters string, actual one is named '{languageCodePart}'";
                Assert.That(languageCodePart, Has.Length.EqualTo(LANGUAGE_CODE_LENGTH), languageCodeErrorMessage);

                var fileEndingPart = parts[2];
                var fileEndingPartErrorMessage = $"The language resource file name has to end with '{APP_RESOURCES_FILE_ENDING}', actual is '{fileEndingPart}'";
                Assert.That(fileEndingPart, Is.EqualTo(APP_RESOURCES_FILE_ENDING), fileEndingPartErrorMessage);
            }
        }

        [Test]
        public void ReadAllLanguageResources_CheckKeysCount_AllKeysCountAreSame()
        {
            // arrange
            var languageDictionary = GetLanguageDictionary();

            // act
            languageDictionary.TryGetValue(DEFAULT_LANGUAGE_CODE, out var defaultLanguageList);
            var defaultCount = defaultLanguageList!.Count;

            foreach (var dictionary in languageDictionary)
            {
                if (dictionary.Key.Equals(DEFAULT_LANGUAGE_CODE))
                {
                    continue;
                }

                var currentCount = dictionary.Value.Count;
                var dictionaryCountErrorMessage = $"The language '{dictionary.Key}' should have {defaultCount} entries but actually has {currentCount} entries";

                // assert
                Assert.That(currentCount, Is.EqualTo(defaultCount), dictionaryCountErrorMessage);
            }
        }

        [Test]
        public void ReadAllLanguageResources_CheckAllValues_NoneOfThemIsEmpty()
        {
            // arrange
            var languageDictionary = GetLanguageDictionary();

            foreach (var dictionary in languageDictionary)
            {
                foreach (var entry in dictionary.Value)
                {
                    // act
                    var entryHasEmptyValueErrorMessage = $"Entry with the name '{entry.Name}' has an empty value in '{dictionary.Key}' language";

                    // assert
                    Assert.That(entry.Value, Is.Not.Empty, entryHasEmptyValueErrorMessage);
                }
            }
        }

        [Test]
        public void ReadAllLanguageResources_CheckOtherLanguages_AllEntriesExistInDefaultLanguage()
        {
            // arrange
            var languageDictionary = GetLanguageDictionary();

            // act
            languageDictionary.TryGetValue(DEFAULT_LANGUAGE_CODE, out var defaultLanguageList);

            foreach (var dictionary in languageDictionary)
            {
                if (dictionary.Key.Equals(DEFAULT_LANGUAGE_CODE))
                {
                    continue;
                }

                foreach (var entry in dictionary.Value)
                {
                    var exists = defaultLanguageList!.Any(l => l.Name.Equals(entry.Name));
                    var entryDoesntExistInDefaultLanguageMessage = $"Entry with the name '{entry.Name}' does not exist in default language but in language '{dictionary.Key}'";

                    // assert
                    Assert.That(exists, entryDoesntExistInDefaultLanguageMessage);
                }
            }
        }
    }
}