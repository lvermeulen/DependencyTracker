using System.Collections.Generic;

namespace DependencyLoader.Git
{
    public class GitConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CloneBaseFolder { get; set; }
        public IEnumerable<string> RepositoryCloneUrls { get; set; }
        public string PathToGit { get; set; }
    }
}
