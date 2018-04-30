using System;

namespace DependencyTracker.GemFileReader
{
    public static class StringExtensions
    {
        public static string TrimStart(this string s, string trim)
        {
            if (!s.StartsWith(trim, StringComparison.OrdinalIgnoreCase))
            {
                return s;
            }

            int index = s.IndexOf(trim, 0, StringComparison.OrdinalIgnoreCase);
            return index >= 0
                ? s.Remove(index, trim.Length)
                : s;
        }

        private static string Without(this string s, string without)
        {
            string result = s;
            if (result.IndexOf(without, StringComparison.OrdinalIgnoreCase) == 0)
            {
                result = result.Remove(0, 1);
            }

            int lastIndex = result.LastIndexOf(without, StringComparison.OrdinalIgnoreCase);
            if (lastIndex > 0)
            {
                result = result.Remove(lastIndex, 1);
            }

            return result;
        }

        public static string WithoutQuotes(this string s)
        {
            if (s.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                return s.Without("\"");
            }

            return s.StartsWith("'", StringComparison.OrdinalIgnoreCase)
                ? s.Without("'")
                : s;
        }
    }
}
