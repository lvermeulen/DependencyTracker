using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dependency.Core;
using Newtonsoft.Json;

namespace DependencyReader.Npm
{
    public class NpmReader : IDependencyReader
    {
        private readonly IEnumerable<FileInfo> _fileInfos;

        public int Count { get; }

        public NpmReader(string path)
        {
            _fileInfos = new DirectoryInfo(path).EnumerateFiles("package.json", SearchOption.AllDirectories);
            Count = _fileInfos.Count();
        }

        public IEnumerable<Dependency.Core.Dependency> Read(Action progress = null)
        {
            var results = new List<Dependency.Core.Dependency>();

            foreach (var fileInfo in _fileInfos)
            {
                string projectName = GetProjectName(fileInfo);
                if (projectName == null)
                {
                    return Enumerable.Empty<Dependency.Core.Dependency>();
                }

                var dependencies = GetDependencies(projectName, fileInfo, progress).ToList();

                results.AddRange(dependencies);
            }

            return results;
        }

        private string GetProjectName(FileInfo fileInfo) => fileInfo?.Directory?.Name;

        private IEnumerable<Dependency.Core.Dependency> GetDependencies(string projectName, FileInfo fileInfo, Action progress = null)
        {
            string json = File.ReadAllText(fileInfo.FullName);
            var package = JsonConvert.DeserializeObject<dynamic>(json);
            string dependenciesNode = Convert.ToString(package.dependencies);
            if (string.IsNullOrEmpty(dependenciesNode))
            {
                return Enumerable.Empty<Dependency.Core.Dependency>();
            }

            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dependenciesNode);
            var results = dict
                .Select(x => new Dependency.Core.Dependency
                {
                    ProjectName = projectName,
                    DependencyId = x.Key,
                    DependencyVersion = Clean(x.Value),
                    DependencyFramework = null
                });

            progress?.Invoke();

            return results;
        }

        private string Clean(string s)
        {
            return s
                .Replace("<", "")
                .Replace("<=", "")
                .Replace(">", "")
                .Replace(">=", "")
                .Replace("~", "")
                .Replace("^", "");
        }
    }
}
