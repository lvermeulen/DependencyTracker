using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Dependency.Core;

namespace DependencyLoader.Git
{
    public class GitLoader : IDependencyLoader
    {
        private readonly GitConfig _config;

        public bool Success { get; }
        public string Location { get; }

        public GitLoader(GitConfig config)
        {
            _config = config;

            if (!Directory.Exists(_config.CloneBaseFolder))
            {
                Directory.CreateDirectory(_config.CloneBaseFolder);
            }

            foreach (string repositoryCloneUrl in _config.RepositoryCloneUrls)
            {
                (bool success, string output) = CloneRepository(repositoryCloneUrl);
                if (!success)
                {
                    return;
                }
            }

            Success = true;
            Location = _config.CloneBaseFolder;
        }

        public void Dispose()
        {
            if (!Directory.Exists(_config.CloneBaseFolder))
            {
                return;
            }

            NormalizeAttributes(_config.CloneBaseFolder);
            Directory.Delete(_config.CloneBaseFolder, true);
        }

        private (bool, string) CloneRepository(string repositoryCloneUrl)
        {
            var repositoryUri = new Uri(repositoryCloneUrl);
            string targetFolder = Path.Combine(_config.CloneBaseFolder, StripInvalidChars(repositoryUri.LocalPath));

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = _config.PathToGit ?? @"C:\Program Files\Git\bin\git.exe",
                    Arguments = $"clone {repositoryCloneUrl} {targetFolder} --depth 1",
                    CreateNoWindow = false
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return (process.ExitCode == 0, output);
            }
        }

        private string StripInvalidChars(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "_"));
        }

        private void NormalizeAttributes(string path)
        {
            var filePaths = Directory.GetFiles(path);
            var subdirectoryPaths = Directory.GetDirectories(path);

            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            foreach (string subdirectoryPath in subdirectoryPaths)
            {
                NormalizeAttributes(subdirectoryPath);
            }
            File.SetAttributes(path, FileAttributes.Normal);
        }
    }
}
