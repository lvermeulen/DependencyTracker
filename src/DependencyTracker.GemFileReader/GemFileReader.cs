using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyTracker.Core;

namespace DependencyTracker.GemFileReader
{
    public class GemFileReader : IDependencyReader
    {
        private readonly IEnumerable<FileInfo> _fileInfos;

        public int Count { get; }

        public GemFileReader(string path)
        {
            _fileInfos = new DirectoryInfo(path).EnumerateFiles("Gemfile", SearchOption.AllDirectories);
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

        private string GetProjectName(FileSystemInfo fileSystemInfo)
        {
            return Path.GetFileName(Path.GetDirectoryName(fileSystemInfo.FullName));
        }

        private IEnumerable<Dependency> GetDependencies(string projectName, FileSystemInfo fileSystemInfo, Action progress = null)
        {
            var results = new List<Dependency>();

            var lines = File.ReadAllLines(fileSystemInfo.FullName);
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (!s.StartsWith("gem ", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                s = s.TrimStart("gem").Trim();
                var pieces = s.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (pieces.Length > 0)
                {
                    string dependencyName = pieces[0]
                        .Trim()
                        .WithoutQuotes();
                    string dependencyVersion = "";
                    if (pieces.Length > 1)
                    {
                        var morePieces = pieces[1].Split(new[] { ',' }, 2);
                        dependencyVersion = morePieces[0]
                            .Trim()
                            .WithoutQuotes()
                            .Trim('[', ']', '\'', '\"', '>', '<', '=', '~', ' ');
                    }

                    results.Add(new Dependency
                    {
                        ProjectName = projectName,
                        Id = dependencyName,
                        Version = dependencyVersion,
                        Framework = null,
                        Type = DependencyTypes.GemFile
                    });
                }
            }

            progress?.Invoke();

            return results;
        }
    }
}
