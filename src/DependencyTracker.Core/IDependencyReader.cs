using System;
using System.Collections.Generic;

namespace DependencyTracker.Core
{
    public interface IDependencyReader
    {
        int Count { get; }

        IEnumerable<Dependency> Read(Action progress = null);
    }
}