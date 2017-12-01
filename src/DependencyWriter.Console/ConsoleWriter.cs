using System.Collections.Generic;
using Dependency.Core;

namespace DependencyWriter.Console
{
    public class ConsoleWriter : IDependencyWriter
    {
        public void Write(IEnumerable<Dependency.Core.Dependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                System.Console.WriteLine($"Project {dependency.ProjectName} depends on {dependency.DependencyId} version {dependency.DependencyVersion} with target framework {dependency.DependencyFramework}");
            }
        }
    }
}
