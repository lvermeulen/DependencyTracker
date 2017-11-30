using CommandLine;

namespace DependencyTracker.Console
{
    public class Options
    {
        [Option("path")]
        public string Path { get; set; }

        [Option("connectionString")]
        public string ConnectionString { get; set; }

        [Option("silent")]
        public bool Silent { get; set; }
    }
}
