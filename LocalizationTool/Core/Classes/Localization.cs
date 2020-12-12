using System.Collections.Generic;
using System.Runtime.Serialization;


namespace LocalizationTool
{
    [DataContract]
    public class Localization
    {
        [DataMember]
        public LocalizationFieldList LocalizationFields { get; set; } = new LocalizationFieldList();

        public Dictionary<string, PoFile> PoFilesMap { get; set; } = new Dictionary<string, PoFile>();

        public string CurrentPoFileLanguage { get; set; } = "";

        public delegate void LocalizationUpdated();

        public event LocalizationUpdated OnLocalizationUpdated;

        public void AddFields(LocalizationField outField)
        {
            LocalizationFields.Add(outField);
        }
        public void AddFields(List<LocalizationField> outLocalizationFields)
        {
            foreach (LocalizationField field in outLocalizationFields)
            {
                AddFields(field);
            }
        }

        public void AddPoFile(PoFile poFile)
        {
            if (CurrentPoFileLanguage == "") CurrentPoFileLanguage = poFile.PoFileLanguage;
            PoFilesMap[poFile.PoFileLanguage] = poFile;
        }

        public PoFile GetCurrentPoFile()
        {
            return PoFilesMap[CurrentPoFileLanguage];
        }
        public bool IsNewPoFileLangugage(string poFileLanguage)
        {
            return CurrentPoFileLanguage != poFileLanguage;
        }

        public void SetLocalizationByPoFile(string poFileLanguage)
        {
            LocalizationFields.Clear();
            CurrentPoFileLanguage = poFileLanguage;
            PoFile poFile = PoFilesMap[poFileLanguage];
            
            foreach (LocalizationField field in poFile.LocalizationFields)
            {
                AddFields(field);
            }

            OnLocalizationUpdated();
        }
        public void ShrinkLocalization(LocalizationFieldStatus fieldStatus)
        {
            LocalizationFields.ShrinkFieldsByStatus(fieldStatus);
            OnLocalizationUpdated();
        }

        public void ClearLocalization()
        {
            CurrentPoFileLanguage = "";

            LocalizationFields.Clear();
            PoFilesMap.Clear();
            OnLocalizationUpdated();
        }
    }
}
