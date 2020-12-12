using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LocalizationTool
{
    [DataContract]
    public class PoFile
    {
        [DataMember]
        public string[] File; //Original file

        public Dictionary<string, int> KeyMap = new Dictionary<string, int>(); // map where key - LocalizationField key, value - index in original file

        public LocalizationFieldList LocalizationFields = new LocalizationFieldList();

        public string PoFileLanguage = "";

        public PoFile(string[] file)
        {
            InitPoFile(file);
        }

        public PoFile()
        {

        }

        public void InitPoFile(string[] file)
        {
            this.File = file;
            string[] StringArr = this.File;

            PoFileLanguage = GetFileLanguage();

            for (int i = 15; i < StringArr.Length; i += 7) // 15 - First key index in po file, +7 - next key index
            {
                string keyString = StringArr[i].Split(new string[] { "#. Key:	" }, StringSplitOptions.None)[1];
                string sourceLocation = StringArr[i + 1].Split(new string[] { "#. SourceLocation:	" }, StringSplitOptions.None)[1];

                string nameSpace = StringArr[i+3].Split(new string[] { "msgctxt " }, StringSplitOptions.None)[1];
                nameSpace = nameSpace.Split(new string[] { "," }, StringSplitOptions.None)[0];
                nameSpace = TrimQuotes(nameSpace);

                string sourceString = StringArr[i + 4].Split(new string[] { "msgid " }, StringSplitOptions.None)[1];
                sourceString = TrimQuotes(sourceString);

                string translationString = StringArr[i + 5].Split(new string[] { "msgstr " }, StringSplitOptions.None)[1];
                translationString = TrimQuotes(translationString);

                LocalizationFields.Add(new LocalizationField(keyString, sourceString, translationString, nameSpace, sourceLocation));
                KeyMap.Add(keyString, i);
            }
        }

        public void ReplaceTranslationByLocalization(Localization localization)
        {
            foreach (LocalizationField field in localization.LocalizationFields)
            {
                if (KeyMap.ContainsKey(field.Key))
                {
                    int currentIndex = KeyMap[field.Key];

                    string translationString = field.TranslationString;
                    translationString = AddQuotes(translationString);

                    File[currentIndex + 5] = "msgstr " + translationString;
                }
            }
        }

        private string GetFileLanguage()
        {
            string[] stringArr = File;
            string[] languageStringFull = stringArr[9].Split(new string[] { "\"Language: " }, StringSplitOptions.None);
            string[] languageStringShort = languageStringFull[1].Split(new string[] { "\\n\"" }, StringSplitOptions.None);

            string language = languageStringShort[0];

            return language;
        }

        private string TrimQuotes(string translationString)
        {
            string quote = "\"";

            if (translationString.StartsWith(quote))
            {
                translationString = translationString.Remove(0, 1);
            }

            if (translationString.EndsWith(quote))
            {
                translationString = translationString.Remove(translationString.Length - 1);
            }

            return translationString;
        }

        private string AddQuotes(string translationString)
        {
            string quote = "\"";

            translationString = translationString.Insert(0, quote);
            translationString = translationString.Insert(translationString.Length, quote);

            return translationString;
        }
    }
}
