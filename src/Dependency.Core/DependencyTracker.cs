using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Dependency.Core.Models;

namespace Dependency.Core
{    public class DependencyTracker
    {
        private readonly IDependencyLoader _loader;
        private readonly IDependencyReader _reader;

        public DependencyTracker(IDependencyLoader loader, IDependencyReader reader, Action progress)
        {
            _loader = loader;
            _reader = reader;
        }

        public RunResult Run()
        {
            var results = new List<Models.Dependency>();

            string location = _loader.Load();
            var di = new DirectoryInfo(location);

            foreach (var fileInfo in di.EnumerateFiles("packages.config", SearchOption.AllDirectories))
            {
                string projectName = GetProjectName(fileInfo.Directory);
                var dependencies = _reader.GetDependencies(projectName, fileInfo.FullName);

                results.AddRange(dependencies);
            }

            return new RunResult
            {
                Projects = results
                    .SelectMany(x => new List<string> { x.ProjectName, x.DependencyId })
                    .Distinct()
                    .Select(x => new Project { Name = x }),

                ProjectDependencies = results
                    .Select(x => new ProjectDependency
                    {
                        From = x.ProjectName,
                        To = x.DependencyId,
                        Version = x.DependencyVersion,
                        TargetFramework = x.DependencyFramework
                    })
            };
        }

        private string GetProjectName(DirectoryInfo directory)
        {
            string firstProjectFileName = directory
                .EnumerateFiles("*.??proj", SearchOption.TopDirectoryOnly)
                .FirstOrDefault()
                ?.FullName;

            var doc = XDocument.Load(firstProjectFileName);
            var xmlns = doc.Root?.Name.Namespace;
            string assemblyName = doc
                .Element(xmlns + "Project")
                ?.Elements(xmlns + "PropertyGroup")
                .Elements(xmlns + "AssemblyName")
                .FirstOrDefault(x => x != null)?.Value;

            return assemblyName;
        }
    }
}
