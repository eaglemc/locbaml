using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BamlLocalization
{
    internal enum TranslationFileType
    {
        CSV,
        TXT,
        XLIFF,
    }

    static class TranslationFileTypeHelpers
    {
        static readonly Dictionary<TranslationFileType, string> TranslationFileTypeExtensions = new Dictionary<TranslationFileType, string>()
        {
            { TranslationFileType.CSV, "csv" },
            { TranslationFileType.TXT, "txt" },
            { TranslationFileType.XLIFF, "xlf" }
        };
        internal static string GetExtension(this TranslationFileType fileType, bool includeDot = true)
        {
            string extension;
            TranslationFileTypeExtensions.TryGetValue(fileType, out extension);
            if ((extension != null) && includeDot)
            {
                return "." + extension;
            }
            else
            {
                // Don't need to prepend a dot or we don't know the extension
                return extension;
            }
        }
    }
}
