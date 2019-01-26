using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Emulators
{
    public static class Extensions
    {
        public static string AsFileParameter(this string fileName) => $"\"{fileName}\"";

        public static string RemoveInvalidChars(this string invalidString)
        {
            return Regex.Replace(invalidString, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
        }
    }
}
