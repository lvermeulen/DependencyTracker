using System;
using System.IO;
using DependencyLoader.Git;
using DependencyReader.NuGet;
using DependencyWriter.Mssql;
using Fclp;

namespace DependencyTrackerConsole
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var options = ParseCommandLine(args);

            string clonePath = options.ClonePath;
            var cloneUrls = File.ReadAllLines(options.CloneUrlsFile);

            // load
            var gitConfig = new GitConfig
            {
                CloneBaseFolder = clonePath,
                UserName = options.UserName,
                Password = options.Password,
                RepositoryCloneUrls = cloneUrls
            };

            Console.WriteLine($"Starting git clone operation on {cloneUrls.Length} repositories...");
            using (var loader = new GitLoader(gitConfig))
            {
                if (!loader.Success)
                {
                    return 1;
                }

                // read
                var reader = new NuGetReader(loader.Location);
                Console.WriteLine($"Reading dependencies for {reader.Count} projects...");
                var dependencies = reader.Read();

                // write
                Console.WriteLine("Writing dependencies to database...");
                var writer = new MssqlWriter(options.ConnectionString);
                writer.Write(dependencies);

                Console.WriteLine("Cleaning up...");
            }

            Console.WriteLine("Done.");
            return 0;
        }

        private static Options ParseCommandLine(string[] args)
        {
            var parser = new FluentCommandLineParser<Options>();

            parser.Setup(arg => arg.UserName)
                .As("userName")
                .Required();

            parser.Setup(arg => arg.Password)
                .As("password")
                .Required();

            parser.Setup(arg => arg.CloneUrlsFile)
                .As("cloneUrlsFile")
                .Required();

            parser.Setup(arg => arg.ClonePath)
                .As("clonePath")
                .Required();

            parser.Setup(arg => arg.ConnectionString)
                .As("connectionString")
                .Required();

            var result = parser.Parse(args);
            if (result.HasErrors)
            {
                DisplayUsage();
            }

            return parser.Object;
        }

        private static void DisplayUsage()
        {
            Console.WriteLine($"Usage: {nameof(DependencyTrackerConsole)} --userName=<git username> --password=<git password> --cloneUrlsFile=<path to file containing git repo urls to clone> --clonePath=<root location of cloned repos> --connectionString=<database connection string>");
            Environment.Exit(1);
        }
    }
}
