using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyTracker.Core;
using Newtonsoft.Json;

namespace DependencyTracker.LibManReader
{
    public class LibManReader : IDependencyReader
    {
        private readonly IEnumerable<FileInfo> _fileInfos;

        public int Count { get; }

        public LibManReader(string path)
        {
            _fileInfos = new DirectoryInfo(path).EnumerateFiles("libman.json", SearchOption.AllDirectories);
            Count = _fileInfos.Count();
        }

        public IEnumerable<Dependency> Read(Action progress = null)
        {
            var results = new List<Dependency>();

            foreach (var fileInfo in _fileInfos)
            {
                string projectName = GetProjectName(fileInfo);
                if (projectName == null)
                {
                    return Enumerable.Empty<Dependency>();
                }

                var dependencies = GetDependencies(projectName, fileInfo, progress).ToList();

                results.AddRange(dependencies);
            }

            return results;
        }

        private string GetProjectName(FileSystemInfo fileInfo)
        {
            var parentFolder = Path.GetDirectoryName(fileInfo.FullName);
            return new DirectoryInfo(parentFolder)
                .EnumerateFiles("*.csproj")
                .Select(x => Path.GetFileNameWithoutExtension(x.Name))
                .FirstOrDefault();
        }

        private IEnumerable<Dependency> GetDependencies(string projectName, FileInfo fileInfo, Action progress = null)
        {
            string json = File.ReadAllText(fileInfo.FullName);
            var libman = JsonConvert.DeserializeObject<dynamic>(json);
            string dependenciesNode = Convert.ToString(libman.libraries);
            if (string.IsNullOrEmpty(dependenciesNode))
            {
                return Enumerable.Empty<Dependency>();
            }

            var deps = JsonConvert.DeserializeObject<List<dynamic>>(dependenciesNode);
            var results = deps
                .Select(x =>
                {
                    string libraryName = x.library.ToString();
                    (string name, string version) = GetLibraryNameAndVersion(libraryName);
                    return new Dependency
                    {
                        ProjectName = projectName,
                        Id = name,
                        Version = version,
                        Framework = null
                    };
                });

            progress?.Invoke();

            return results;
        }

        private (string name, string version) GetLibraryNameAndVersion(string s)
        {
            var pieces = s.Split('@');
            return (pieces[0], pieces[1]);
        }
    }
}
