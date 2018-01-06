using System;

namespace DependencyTracker.Core
{
    public interface IDependencyLoader : IDisposable
    {
        bool Success { get; }
        string Location { get; }
    }
}
