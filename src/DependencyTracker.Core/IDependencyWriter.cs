using System.Collections.Generic;

namespace DependencyTracker.Core
{
    public interface IDependencyWriter
    {
        void Write(IEnumerable<Dependency> dependencies);
    }
}