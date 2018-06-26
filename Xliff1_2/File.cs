using System;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    public class File
    {
        [XmlAttribute("source-language")]
        public string SourceLanguage { get; set; }

        [XmlAttribute("target-language")]
        public string TargetLanguage { get; set; }

        [XmlAttribute("original")]
        public string Original { get; set; }

        [XmlElement("header")]
        public Header Header { get; set; } = new Header();

        [XmlElement("body")]
        public Body Body { get; set; } = new Body();

        #region Constants

        [XmlAttribute("datatype")]
        public string Datatype { get; set; } = "xml";

        [XmlAttribute("tool-id")]
        public string ToolId { get; set; } = "locbaml";

        [XmlAttribute("product-name")]
        public string ProductName { get; set; } = "n/a";

        [XmlAttribute("product-version")]
        public string ProductVersion { get; set; } = "n/a";

        [XmlAttribute("build-num")]
        public string BuildNumber { get; set; } = "n/a";

        #endregion
    }
}
