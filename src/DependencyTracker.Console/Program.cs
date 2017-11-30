using System;
using System.Collections.Generic;
using CommandLine;
using Dependency.Core;
using Dependency.Loader;
using Dependency.Reader;
using Dependency.Writer;
using ShellProgressBar;

namespace DependencyTracker.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // parse
            var options = new Options();
            Parser.Default.ParseArguments(() => new Options(), args)
                .WithParsed(x => options = x);
            if (options.Path == null || options.ConnectionString == null)
            {
                if (!options.Silent)
{
                    DisplayUsage();
                }

                Environment.Exit(1);
            }

            // load
            var loader = new LocationLoader(options.Path);
            string path = loader.Load();

            using (var progressBar = options.Silent ? null : new ProgressBar(1, "", new ProgressBarOptions { ForegroundColor = ConsoleColor.Gray, ProgressCharacter = '-' }))
            {
                // read
                if (progressBar != null)
                {
                    progressBar.Message = "Thinking";
                }
                var reader = new NuGetReader(path);

                if (progressBar != null)
                {
                    progressBar.MaxTicks = reader.Count;
                    progressBar.Message = "Reading dependencies";
                }
                var dependencies = reader.Read(() => { progressBar?.Tick(); });

                // write
                if (progressBar != null)
                {
                    progressBar.Message = "Writing";
                }
                var writer = new MsSqlWriter(options.ConnectionString);
                Write(writer, dependencies);

                // close
                if (progressBar != null)
                {
                    System.Console.Clear();
                    System.Console.WriteLine($"{reader.Count} dependencies updated");
                }
            }
        }

        private static void Write(IDependencyWriter writer, IEnumerable<Dependency.Core.Dependency> dependencies)
        {
            writer.PreWrite();
            writer.Write(dependencies);
            writer.PostWrite();
        }

        private static void DisplayUsage()
        {
            System.Console.WriteLine("Usage: DependencyTracker.Console --path=<root folder to search> --connectionString=<database connection string>");
        }
    }
}
