using System.Collections.Generic;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    public class Body
    {
        [XmlElement("group")]
        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
