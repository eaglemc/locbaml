using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    [XmlRoot(ElementName = "xliff", Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
    public class XliffObject
    {
        [XmlAttribute("version")]
        public string Version { get; set; } = "1.2";

        [XmlElement("file")]
        public List<File> Files { get; set; } = new List<File>();

        [XmlAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; } = "urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd";

        public void Serialize(System.IO.Stream stream)
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            XmlSerializer serializer = new XmlSerializer(typeof(XliffObject));
            serializer.Serialize(stream, this, namespaces);
        }

        public static XliffObject Deserialize(System.IO.Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XliffObject));
            return (XliffObject)serializer.Deserialize(stream);
        }
    }
}
