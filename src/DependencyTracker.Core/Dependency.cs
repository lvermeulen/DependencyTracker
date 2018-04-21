namespace DependencyTracker.Core
{
    public class Dependency
    {
        public string ProjectName { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public string Framework { get; set; }
        public DependencyTypes Type { get; set; }
    }
}