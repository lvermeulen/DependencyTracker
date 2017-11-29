using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dependency.Core;

namespace Dependency.Reader
{
    public class NuGetReader : IDependencyReader
    {
        public IEnumerable<Core.Models.Dependency> GetDependencies(string projectName, string fileName)
        {
            //
            // file is packages.config
            //

            var doc = XDocument.Load(fileName);
            var results = doc
                .Element("packages")
                ?.Elements("package")
                .Select(x => new Core.Models.Dependency
                {
                    ProjectName = projectName,
                    DependencyId = x?.Attribute("id")?.Value,
                    DependencyVersion = x?.Attribute("version")?.Value,
                    DependencyFramework = x?.Attribute("targetFramework")?.Value
                });

            return results;
        }
    }
}
