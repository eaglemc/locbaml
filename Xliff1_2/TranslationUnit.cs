using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    public class TranslationUnit
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("source")]
        public string Source { get; set; }

        [XmlElement("target")]
        public Target Target { get; set; } = new Target() { State = TranslationState.New };

        [XmlElement("note")]
        public List<Note> Notes { get; set; } = new List<Note>();

        #region constant

        // TBD: is this constant??
        [XmlAttribute("translate")]
        public string Translate { get; set; } = "yes";

        // to get the attribute xml:space="preserve"
        [XmlAttribute("space", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Space { get; set; } = "preserve";

        #endregion
    }
}
