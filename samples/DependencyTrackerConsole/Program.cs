﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DbUp;
using DependencyTracker.Core;
using DependencyTracker.GemFileReader;
using DependencyTracker.GitLoader;
using DependencyTracker.LibManReader;
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
                    Console.WriteLine("Loader failed to load; exiting");
                    return 1;
                }

                // read
                var dependencies = ReadDependencies(new NuGetReader(loader.Location))
                    .Union(ReadDependencies(new NpmReader(loader.Location))
                    .Union(ReadDependencies(new LibManReader(loader.Location))
                    .Union(ReadDependencies(new GemFileReader(loader.Location)))));

                // write
                Console.WriteLine("Writing dependencies to database...");
                if (!CreateDatabase(options.ConnectionString))
                {
                    return -1;
                }
                var writer = new MssqlWriter(options.ConnectionString);
                writer.Write(dependencies);

                Console.WriteLine("Cleaning up...");
            }

            Console.WriteLine("Done.");
            return 0;
        }

        private static IEnumerable<Dependency> ReadDependencies(IDependencyReader dependencyReader)
        {
            if (dependencyReader == null)
            {
                throw new ArgumentNullException(nameof(dependencyReader));
            }

            Console.WriteLine($"Reading dependencies for {dependencyReader.Count} projects...");
            return dependencyReader.Read();
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

        private static bool CreateDatabase(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);
            var result = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build()
                .PerformUpgrade();

            if (result.Successful)
            {
                return true;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
#if DEBUG
            Console.ReadLine();
#endif
            return false;
        }
    }
}
