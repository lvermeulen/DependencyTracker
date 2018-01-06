using System;
using System.IO;
using System.Linq;
using DependencyTracker.Core;

namespace DependencyTracker.GitLoader
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
                (bool success, string _) = CloneRepository(repositoryCloneUrl);
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

            try
            {
                NormalizeAttributes(_config.CloneBaseFolder);
                Directory.Delete(_config.CloneBaseFolder, true);
            }
            catch
            {
                // we don't care about exceptions here
            }
        }

        private (bool, string) CloneRepository(string repositoryCloneUrl)
        {
            var repositoryUri = new Uri(repositoryCloneUrl);
            string targetFolder = Path.Combine(_config.CloneBaseFolder, StripInvalidChars(repositoryUri.LocalPath));

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo = new System.Diagnostics.ProcessStartInfo()
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
