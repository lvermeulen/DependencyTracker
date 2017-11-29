using Dependency.Core.Models;

namespace Dependency.Core
{
    public interface IDependencyWriter
    {
        void PreWrite();
        void Write(RunResult runResult);
        void PostWrite();
    }
}