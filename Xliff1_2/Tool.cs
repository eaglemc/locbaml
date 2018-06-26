using System;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    public class Tool
    {
        [XmlAttribute("tool-id")]
        public string ToolId { get; set; } = "locbaml";

        [XmlAttribute("tool-name")]
        public string ToolName { get; set; } = "locbaml";

        [XmlAttribute("tool-version")]
        public string ToolVersion { get; set; } = "0.0.0.0";
            
        //tool-company="Microsoft"
    }
}
