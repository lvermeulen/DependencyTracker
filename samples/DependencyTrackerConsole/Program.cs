using System;
using System.IO;
using System.Linq;
using DependencyTracker.GitLoader;
using DependencyTracker.MssqlWriter;
using DependencyTracker.NpmReader;
using DependencyTracker.NuGetReader;
using Fclp;

namespace DependencyTrackerConsole
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, a) => { Console.WriteLine($"Unhandled exception: {((Exception)a.ExceptionObject).Message}"); };

            var options = ParseCommandLine(args);

            string clonePath = options.ClonePath;
            var cloneUrls = File.ReadAllLines(options.CloneUrlsFile);

            // load
            var gitConfig = new GitConfig
            {
                CloneBaseFolder = clonePath,
                UserName = options.UserName,
                Password = options.Password,
                RepositoryCloneUrls = cloneUrls,
                ShallowClone = true
            };

            Console.WriteLine($"Starting git clone operation on {cloneUrls.Length} repositories...");
            using (var loader = new GitLoader(gitConfig))
            {
                if (!loader.Success)
                {
                    return 1;
                }

                // read
                var nuGetReader = new NuGetReader(loader.Location);
                Console.WriteLine($"Reading nuget dependencies for {nuGetReader.Count} projects...");
                var nugetDependencies = nuGetReader.Read().ToList();

                var npmReader = new NpmReader(loader.Location);
                Console.WriteLine($"Reading npm dependencies for {npmReader.Count} projects...");
                var npmDependencies = npmReader.Read();

                var dependencies = nugetDependencies.Union(npmDependencies);

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
