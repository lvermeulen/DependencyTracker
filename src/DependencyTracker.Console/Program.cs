using System.Linq;
using Dependency.Core;
using Dependency.Core.Models;
using Dependency.Loader;
using Dependency.Reader;
using Dependency.Writer;

namespace DependencyTracker.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var loader = new LocationLoader("E:\\bbrepo");
            var reader = new NuGetReader();
            var writer = new MsSqlWriter("Server=OB07SQL01;Database=Dependencies;User Id=dotnet;Password=domoware;");

            var tracker = new Dependency.Core.DependencyTracker(loader, reader, () => { });
            var result = tracker.Run();

            Write(writer, result);

            System.Console.WriteLine($"{result.ProjectDependencies.Count()} dependencies updated");
        }

        private static void Write(IDependencyWriter writer, RunResult runResult)
        {
            writer.PreWrite();
            writer.Write(runResult);
            writer.PostWrite();
        }
    }
}
