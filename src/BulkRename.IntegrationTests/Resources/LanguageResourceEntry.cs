namespace BulkRename.IntegrationTests.Resources
{
    internal class LanguageResourceEntry
    {
        public LanguageResourceEntry(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}