using System.Globalization;

namespace ReadMySongs.Utilities
{
    public static class Helper
    {
        private static CultureInfo Culture = CultureInfo.InvariantCulture;

        public static bool ContainsIgnoreCase(this string original, string text)
        {
            return Culture.CompareInfo.IndexOf(original, text, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
