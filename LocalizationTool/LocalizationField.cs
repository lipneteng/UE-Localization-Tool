using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public enum LocalizationFieldStatus
    {
        Idle,
        New,
        Changed,
        NoTranslation
    }

    public class LocalizationFields : List<LocalizationField>
    {

        public Dictionary<string, LocalizationFieldStatus> LocalizationFieldsStatusMap = new Dictionary<string, LocalizationFieldStatus>();

        public new void Add(LocalizationField localizationField)
        {
            bool isEmptyTranslation = localizationField.TranslationString == "";
            LocalizationFieldStatus localizationFieldStatus = isEmptyTranslation ? LocalizationFieldStatus.NoTranslation : LocalizationFieldStatus.New;

            foreach (LocalizationField inField in this)
            {
                if (inField.Key == localizationField.Key)
                {
                    if (inField.TranslationString == localizationField.TranslationString) localizationFieldStatus = LocalizationFieldStatus.Idle;
                    else localizationFieldStatus = LocalizationFieldStatus.Changed;

                    if (isEmptyTranslation) localizationFieldStatus = LocalizationFieldStatus.NoTranslation;

                    inField.TranslationString = localizationField.TranslationString;
                    LocalizationFieldsStatusMap[inField.Key] = localizationFieldStatus;
                    return;
                }
            }
            LocalizationFieldsStatusMap.Add(localizationField.Key, localizationFieldStatus);
            base.Add(localizationField);
        }

        public new bool Remove(LocalizationField localizationField)
        {
            if (!Contains(localizationField)) return false;

            LocalizationFieldsStatusMap.Remove(localizationField.Key);
            base.Remove(localizationField);
            return true;
        }

        public new void Clear()
        {
            LocalizationFieldsStatusMap.Clear();
            base.Clear();
        }

        public LocalizationField this[string key]
        {
            get
            {
                foreach (LocalizationField localizationField in this)
                {
                    if (localizationField.Key == key) return localizationField;
                }

                return new LocalizationField();
            }
        }

        public void ShrinkFieldsByStatus(LocalizationFieldStatus fieldStatus)
        {
            List<LocalizationField> localizationFields = this.ToList();

            foreach (LocalizationField field in localizationFields)
            {
                if (LocalizationFieldsStatusMap[field.Key] != fieldStatus)
                {
                    this.Remove(field);
                }
            }
        }

        public LocalizationFieldStatus GetFieldStatusByKey(string key)
        {
            return LocalizationFieldsStatusMap[key];
        }
    }

}
