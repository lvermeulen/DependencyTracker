using System;
using System.Collections.Generic;
using CommandLine;
using Dependency.Core;
using DependencyLoader.Location;
using DependencyReader.NuGet;
using DependencyWriter.Mssql;

namespace DependencyTrackerConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var options = ParseCommandLine(args);

            // load
            var loader = new LocationLoader(options.Path);
            string path = loader.Load();

            // read
            var reader = new NuGetReader(path);
            var dependencies = reader.Read();

            // write
            var writer = new MssqlWriter(options.ConnectionString);
            Write(writer, dependencies);

            Console.WriteLine($"{reader.Count} dependencies updated");
        }

        private static Options ParseCommandLine(string[] args)
        {
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

            return options;
        }

        private static void Write(IDependencyWriter writer, IEnumerable<Dependency.Core.Dependency> dependencies)
        {
            writer.PreWrite();
            writer.Write(dependencies);
            writer.PostWrite();
        }

        private static void DisplayUsage()
        {
            Console.WriteLine($"Usage: {nameof(DependencyTrackerConsole)} --path=<root folder to search> --connectionString=<database connection string>");
        }
    }
}
