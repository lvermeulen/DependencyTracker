using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyTracker.Core;
using Newtonsoft.Json;

namespace DependencyTracker.NpmReader
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

        private string GetProjectName(FileInfo fileInfo)
        {
            string json = File.ReadAllText(fileInfo.FullName);
            var package = JsonConvert.DeserializeObject<dynamic>(json);

            return Convert.ToString(package.name);
        }

        private IEnumerable<Dependency> GetDependencies(string projectName, FileInfo fileInfo, Action progress = null)
        {
            string json = File.ReadAllText(fileInfo.FullName);
            var package = JsonConvert.DeserializeObject<dynamic>(json);
            string dependenciesNode = Convert.ToString(package.dependencies);
            if (string.IsNullOrEmpty(dependenciesNode))
            {
                return Enumerable.Empty<Dependency>();
            }

            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dependenciesNode);
            var results = dict
                .Select(x => new Dependency
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
