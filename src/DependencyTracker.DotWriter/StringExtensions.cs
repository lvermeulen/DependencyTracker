namespace DependencyTracker.DotWriter
{
    public static class StringExtensions
    {
        public static string NormalizeDotName(this string s) => s.Replace(".", "_");
    }
}
