using System;
using System.Windows.Markup.Localizer;

namespace BamlLocalization
{
    interface ITranslationWriter : IDisposable
    {
        void WriteResource(string bamlStreamName, string resourceKey, BamlLocalizableResource resource);
    }
}
