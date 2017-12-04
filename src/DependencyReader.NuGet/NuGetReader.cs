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
            _fileInfos = new DirectoryInfo(path).EnumerateFiles("*.??proj", SearchOption.AllDirectories);
            Count = _fileInfos.Count();
        }

        public IEnumerable<Dependency.Core.Dependency> Read(Action progress = null)
        {
            var results = new List<Dependency.Core.Dependency>();

            foreach (var fileInfo in _fileInfos)
            {
                string projectName = GetProjectName(fileInfo.FullName);
                if (projectName == null)
                {
                    return Enumerable.Empty<Dependency.Core.Dependency>();
                }

                var dependencies = GetDependencies(projectName, fileInfo, progress).ToList();

                results.AddRange(dependencies);
            }

            return results;
        }

        private string GetProjectName(string fileName)
        {
            var doc = XDocument.Load(fileName);
            var xmlns = doc.Root?.Name.Namespace;
            string assemblyName = doc
                .Element(xmlns + "Project")
                ?.Elements(xmlns + "PropertyGroup")
                .Elements(xmlns + "AssemblyName")
                .FirstOrDefault(x => x != null)
                ?.Value;

            return assemblyName ?? Path.GetFileNameWithoutExtension(fileName);
        }

        private IEnumerable<Dependency.Core.Dependency> GetDependencies(string projectName, FileInfo fileInfo, Action progress = null)
        {
            var doc = XDocument.Load(fileInfo.FullName);
            var results = doc
                .Element("Project")
                ?.Elements("ItemGroup")
                .Elements("PackageReference")
                .Select(x => new Dependency.Core.Dependency
                {
                    ProjectName = projectName,
                    DependencyId = x?.Attribute("Include")?.Value,
                    DependencyVersion = x?.Attribute("Version")?.Value,
                    DependencyFramework = null
                }) ?? GetPackagesConfigDependencies(projectName, fileInfo.Directory);

            progress?.Invoke();

            return results;
        }

        private IEnumerable<Dependency.Core.Dependency> GetPackagesConfigDependencies(string projectName, DirectoryInfo directory)
        {
            string fileName = directory
                .EnumerateFiles("packages.config", SearchOption.TopDirectoryOnly)
                .FirstOrDefault()
                ?.FullName;
            if (fileName == null)
            {
                return Enumerable.Empty<Dependency.Core.Dependency>();
            }

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

            return results;
        }
    }
}
