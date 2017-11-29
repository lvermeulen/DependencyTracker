using System.Collections.Generic;

namespace Dependency.Core
{
    public interface IDependencyReader
    {
        IEnumerable<Models.Dependency> GetDependencies(string projectName, string fileName);
    }
}