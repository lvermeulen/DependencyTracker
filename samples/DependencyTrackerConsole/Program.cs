using System;
using System.IO;
using CommandLine;
using DependencyLoader.Git;
using DependencyReader.NuGet;
using DependencyWriter.Mssql;

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
            using (var loader = new GitLoader(gitConfig))
            {
                if (!loader.Success)
                {
                    return 1;
                }

                // read
                var reader = new NuGetReader(loader.Location);
                var dependencies = reader.Read();

                // write
                var writer = new MssqlWriter(options.ConnectionString);
                writer.Write(dependencies);

                Console.WriteLine($"{reader.Count} files read");
                return 0;
            }
        }

        private static Options ParseCommandLine(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArguments(() => new Options(), args)
                .WithParsed(x => options = x);
            if (options.ConnectionString == null)
            {
                if (!options.Silent)
                {
                    DisplayUsage();
                }

                Environment.Exit(1);
            }

            return options;
        }

        private static void DisplayUsage()
        {
            Console.WriteLine($"Usage: {nameof(DependencyTrackerConsole)} --path=<root folder to search> --connectionString=<database connection string>");
        }
    }
}
