using System.Collections.Generic;

namespace Dependency.Core.Models
{    public class RunResult
    {
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<ProjectDependency> ProjectDependencies { get; set; }
    }
}
