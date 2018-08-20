using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyTracker.Core;
using DotNetGraph;

namespace DependencyTracker.DotWriter
{
    public class DotWriter : IDependencyWriter
    {
        private readonly string _name;
        private readonly string _outputFileName;

        public DotWriter(string name, string outputFileName)
        {
            _name = name;
            _outputFileName = outputFileName;
        }

        public void Write(IEnumerable<Dependency> dependencies)
        {
            var graph = new DotGraph(_name, directed: true);

            var dependencyList = dependencies.ToList();
            var projectMap = WriteProjects(dependencyList, graph);
            WriteProjectDependencies(dependencyList, projectMap, graph);

            var dot = graph.Compile();
            File.WriteAllText(_outputFileName, dot);
        }

        private IDictionary<string, DotNode> WriteProjects(IEnumerable<Dependency> dependencies, DotGraph graph)
        {
            var projects = dependencies
                .SelectMany(x => new List<string> { x.ProjectName, x.Id })
                .Distinct()
                .Select(x =>
                {
                    var node = new DotNode(x.NormalizeDotName())
                    {
                        Shape = DotNodeShape.Ellipse,
                        Label = x,
                        FillColor = DotColor.Lightgrey,
                        FontColor = DotColor.Black,
                        Style = DotNodeStyle.Default,
                        Height = 0.5f
                    };

                    graph.Add(node);

                    return new
                    {
                        Name = x,
                        Node = node
                    };
                });

            return projects.ToDictionary(x => x.Name.NormalizeDotName(), x => x.Node);
        }

        private void WriteProjectDependencies(IEnumerable<Dependency> dependencies, IDictionary<string, DotNode> projectMap, DotGraph graph)
        {
            dependencies
                .ToList()
                .ForEach(x =>
                {
                    graph.Add(new DotArrow(projectMap[x.ProjectName.NormalizeDotName()], projectMap[x.Id.NormalizeDotName()])
                    {
                        ArrowHeadShape = DotArrowShape.Normal,
                    });
                });
        }
    }
}
