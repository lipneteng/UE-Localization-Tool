namespace LocalizationTool
{
    public class LocalizationField
    {
        public string Key { get; set; }
        public string SourceString { get; set; }
        public string TranslationString { get; set; }
        public string NameSpace { get; set; }
        public string SourceLocation { get; set; }

        public LocalizationField(string key, string sourceString, string translationString, string nameSpace, string sourceLocation)
        {
            Key = key;
            SourceString = sourceString;
            TranslationString = translationString;
            NameSpace = nameSpace;
            SourceLocation = sourceLocation;
        }

        public LocalizationField()
        {
            Key = "No key";
        }

        public LocalizationField(string key)
        {
            Key = key;
            SourceString = "No source text";
            TranslationString = "No translation text";
        }
    }
}
