using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BulkWriter;
using Dapper;
using Dependency.Core;

namespace Dependency.Writer
{    public class MsSqlWriter : IDependencyWriter
    {
        private readonly string _connectionString;

        public MsSqlWriter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void PreWrite()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Execute("delete from project");
            connection.Execute("delete from projectdependency");
        }

        public void Write(IEnumerable<Core.Dependency> dependencies)
        {
            var dependencyList = dependencies.ToList();
            var projectMap = WriteProjects(dependencyList);
            WriteProjectDependencies(dependencyList, projectMap);
        }

        private IDictionary<string, int> WriteProjects(List<Core.Dependency> dependencies)
        {
            var projectIntCounter = new IntCounter(-1);
            var projects = dependencies
                .SelectMany(x => new List<string> { x.ProjectName, x.DependencyId })
                .Distinct()
                .Select(x => new Project
                {
                    Id = projectIntCounter.Next(),
                    Name = x
                })
                .ToList();

            using (var bulkWriter = new BulkWriter<Project>(_connectionString))
            {
                bulkWriter.WriteToDatabase(projects);
            }

            return projects.ToDictionary(x => x.Name, x => x.Id);
        }

        private void WriteProjectDependencies(List<Core.Dependency> dependencies, IDictionary<string, int> projectMap)
        {
            var projectDependencyIntCounter = new IntCounter(-1);
            var projectDependencies = dependencies
                .Select(x => new ProjectDependency
                {
                    Id = projectDependencyIntCounter.Next(),
                    ProjectFromId = projectMap[x.ProjectName],
                    ProjectToId = projectMap[x.DependencyId],
                    Version = x.DependencyVersion,
                    TargetFramework = x.DependencyFramework
                });

            using (var bulkWriter = new BulkWriter<ProjectDependency>(_connectionString))
            {
                bulkWriter.WriteToDatabase(projectDependencies);
            }
        }

        public void PostWrite()
        {
            // nothing to do
        }
    }
}
