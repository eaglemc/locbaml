using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    public class Group
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("datatype")]
        public string DataType { get; set; }

        [XmlElement("trans-unit")]
        public List<TranslationUnit> TranslationUnits { get; set; } = new List<TranslationUnit>();
    }
}
