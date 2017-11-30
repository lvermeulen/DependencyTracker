using System;
using System.Collections.Generic;

namespace Dependency.Core
{
    public interface IDependencyReader
    {
        int Count { get; }

        IEnumerable<Dependency> Read(Action progress = null);
    }
}