using System;

namespace Dependency.Core
{
    public interface IDependencyLoader : IDisposable
    {
        bool Success { get; }
        string Location { get; }
    }
}
