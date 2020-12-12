using System.Collections.Generic;
using System.Linq;


namespace LocalizationTool
{
    public enum LocalizationFieldStatus
    {
        Idle,
        New,
        Changed,
        NoTranslation
    }

    public class LocalizationFieldList : List<LocalizationField>
    {
        public Dictionary<string, LocalizationFieldStatus> LocalizationFieldsStatusMap = new Dictionary<string, LocalizationFieldStatus>();

        public new void Add(LocalizationField localizationField)
        {
            bool isEmptyTranslation = localizationField.TranslationString == "";
            LocalizationFieldStatus localizationFieldStatus = isEmptyTranslation ? LocalizationFieldStatus.NoTranslation : LocalizationFieldStatus.New;

            foreach (LocalizationField inField in this) //TODO: not very pretty code, need refactor
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
