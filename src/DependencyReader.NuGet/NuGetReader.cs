using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Dependency.Core;

namespace DependencyReader.NuGet
{
    public class NuGetReader : IDependencyReader
    {
        private readonly IEnumerable<FileInfo> _fileInfos;

        public int Count { get; }

        public NuGetReader(string path)
        {
            _fileInfos = new DirectoryInfo(path).EnumerateFiles("packages.config", SearchOption.AllDirectories);
            Count = _fileInfos.Count();
        }

        public IEnumerable<Dependency.Core.Dependency> Read(Action progress = null)
        {
            var results = new List<Dependency.Core.Dependency>();

            foreach (var fileInfo in _fileInfos)
            {
                string projectName = GetProjectName(fileInfo.Directory);
                var dependencies = GetDependencies(projectName, fileInfo.FullName, progress).ToList();

                results.AddRange(dependencies);
            }

            return results;
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

        private IEnumerable<Dependency.Core.Dependency> GetDependencies(string projectName, string fileName, Action progress = null)
        {
            //
            // file is packages.config
            //

            var doc = XDocument.Load(fileName);
            var results = doc
                .Element("packages")
                ?.Elements("package")
                .Select(x => new Dependency.Core.Dependency
                {
                    ProjectName = projectName,
                    DependencyId = x?.Attribute("id")?.Value,
                    DependencyVersion = x?.Attribute("version")?.Value,
                    DependencyFramework = x?.Attribute("targetFramework")?.Value
                });

            progress?.Invoke();

            return results;
        }
    }
}
