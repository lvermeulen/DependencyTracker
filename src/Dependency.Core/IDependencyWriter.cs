using System.Collections.Generic;

namespace Dependency.Core
{
    public interface IDependencyWriter
    {
        void Write(IEnumerable<Dependency> dependencies);
    }
}