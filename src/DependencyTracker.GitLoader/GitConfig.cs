using System.Collections.Generic;

namespace DependencyTracker.GitLoader
{
    public class GitConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CloneBaseFolder { get; set; }
        public IEnumerable<string> RepositoryCloneUrls { get; set; }
        public string PathToGit { get; set; }
        public bool ShallowClone { get; set; }
        public bool DoNotDeleteRepositoriesOnDispose { get; set; }
    }
}
