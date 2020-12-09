using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using System.Windows.Media;

namespace LocalizationTool
{
    [DataContract]
    public class Localization
    {
        [DataMember]
        public LocalizationFields LocalizationFields { get; set; } = new LocalizationFields();

        public PoFile PoFile { get; set; }

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

        public void ShrinkLocalization(LocalizationFieldStatus fieldStatus)
        {
            LocalizationFields.ShrinkFieldsByStatus(fieldStatus);
            OnLocalizationUpdated();
        }

        public void ClearLocalization()
        {
            LocalizationFields.Clear();
            PoFile = new PoFile();
            OnLocalizationUpdated();
        }
    }
}
