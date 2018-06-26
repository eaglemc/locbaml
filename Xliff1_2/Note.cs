using System;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    public class Note
    {
        [XmlAttribute("from")]
        public string From { get; set; }

        [XmlAttribute("annotates")]
        public string Annotates { get; set; } = "source";

        [XmlAttribute("priority")]
        public int Priority { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
