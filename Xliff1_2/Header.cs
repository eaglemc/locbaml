using System;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    public class Header
    {
        [XmlElement("tool")]
        public Tool Tool { get; set; } = new Tool();
    }
}
