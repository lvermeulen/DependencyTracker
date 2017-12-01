using CommandLine;

namespace DependencyTrackerConsole
{
    public class Options
    {
        [Option("path")]
        public string Path { get; set; }

        [Option("connectionString")]
        public string ConnectionString { get; set; }

        [Option("silent")]
        public bool Silent { get; set; }

        [Option("userName")]
        public string UserName { get; set; }

        [Option("password")]
        public string Password { get; set; }

        [Option("cloneUrlsFile")]
        public string CloneUrlsFile { get; set; }

        [Option("clonePath")]
        public string ClonePath { get; set; }
    }
}
