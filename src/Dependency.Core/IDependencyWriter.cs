using System.Collections.Generic;

namespace Dependency.Core
{
    public interface IDependencyWriter
    {
        void PreWrite();
        void Write(IEnumerable<Dependency> dependencies);
        void PostWrite();
    }
}