using System;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    [XmlRoot("target")]
    public class Target
    {
        [XmlAttribute("state")]
        public TranslationState State { get; set; } = TranslationState.New;

        [XmlText]
        public string Content { get; set; }
    }
}
