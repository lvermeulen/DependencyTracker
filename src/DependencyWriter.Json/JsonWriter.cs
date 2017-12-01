using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dependency.Core;
using Newtonsoft.Json;

namespace DependencyWriter.Json
{
    public class JsonWriter : IDependencyWriter
    {
        private readonly string _fileName;

        public JsonWriter(string fileName)
        {
            _fileName = fileName;    
        }

        public void Write(IEnumerable<Dependency.Core.Dependency> dependencies)
        {
            var dependencyList = dependencies.ToList();
            var nodes = dependencyList
                .SelectMany(x => new List<string> { x.ProjectName, x.DependencyId })
                .Distinct()
                .ToList();

            var links = dependencyList
                .Select(x => new 
                {
                    source = x.ProjectName,
                    target = x.DependencyId,
                    version = x.DependencyVersion,
                    targetFramework = x.DependencyFramework
                });

            string json = JsonConvert.SerializeObject(new
            {
                nodes,
                links
            });

            File.WriteAllText(_fileName, json, Encoding.UTF8);
        }
    }
}
