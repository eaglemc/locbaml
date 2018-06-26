using System.Collections.Generic;
using System.Globalization;

namespace BamlLocalization
{
    internal enum TranslationFileType
    {
        NONE,
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
        internal static TranslationFileType GetTranslationFileTypeFromExtension(string extension)
        {
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }
            foreach (var pair in TranslationFileTypeExtensions)
            {
                if (string.Compare(extension, pair.Value, true, CultureInfo.InvariantCulture) == 0)
                {
                    return pair.Key;
                }
            }
            return TranslationFileType.NONE;
        }
    }
}
