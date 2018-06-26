using System;
using System.Xml.Serialization;

namespace BamlLocalization.Xliff1_2
{
    [Serializable]
    public enum TranslationState
    {
        [XmlEnum("new")]
        New,
        [XmlEnum("needs-review-translation")]
        NeedsReview,
        [XmlEnum("translated")]
        Translated,
        [XmlEnum("final")]
        Final
    }
}
