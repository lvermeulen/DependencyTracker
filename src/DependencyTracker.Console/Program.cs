using System.Collections.Generic;
using Dependency.Core;
using Dependency.Loader;
using Dependency.Reader;
using Dependency.Writer;
using ShellProgressBar;

namespace DependencyTracker.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var loader = new LocationLoader("E:\\bbrepo");
            string path = loader.Load();

            var reader = new NuGetReader(path);
            IEnumerable<Dependency.Core.Dependency> dependencies;
            using (var progressBar = new ProgressBar(reader.Count, "Reading dependencies"))
            {
                dependencies = reader.Read(() => progressBar.Tick());
            }

            var writer = new MsSqlWriter("Server=OB07SQL01;Database=Dependencies;User Id=dotnet;Password=domoware;");
            Write(writer, dependencies);

            System.Console.WriteLine($"{reader.Count} dependencies updated");
#if DEBUG
            System.Console.ReadLine();
#endif
        }

        private static void Write(IDependencyWriter writer, IEnumerable<Dependency.Core.Dependency> dependencies)
        {
            writer.PreWrite();
            writer.Write(dependencies);
            writer.PostWrite();
        }
    }
}
