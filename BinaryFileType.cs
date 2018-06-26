using System.Collections.Generic;
using System.Globalization;

namespace BamlLocalization
{
    internal enum BinaryFileType
    {
        NONE = 0,
        BAML,
        RESOURCES,
        DLL,
        EXE,
    }

    static class BinaryFileTypeHelper
    {
        static readonly Dictionary<BinaryFileType, string> InputFileTypeExtensions = new Dictionary<BinaryFileType, string>()
        {
            { BinaryFileType.BAML, "baml" },
            { BinaryFileType.DLL, "dll" },
            { BinaryFileType.EXE, "exe" },
            { BinaryFileType.RESOURCES, "resources" }
        };
        internal static string GetExtension(this BinaryFileType fileType, bool includeDot = true)
        {
            string extension;
            InputFileTypeExtensions.TryGetValue(fileType, out extension);
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
        internal static BinaryFileType GetBinaryFileTypeFromExtension(string extension)
        {
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }
            foreach (var pair in InputFileTypeExtensions)
            {
                if (string.Compare(extension, pair.Value, true, CultureInfo.InvariantCulture) == 0)
                {
                    return pair.Key;
                }
            }
            return BinaryFileType.NONE;
        }
    }
}
