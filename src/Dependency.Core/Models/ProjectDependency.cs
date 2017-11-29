namespace Dependency.Core.Models
{
    public class ProjectDependency
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Version { get; set; }
        public string TargetFramework { get; set; }
    }
}
