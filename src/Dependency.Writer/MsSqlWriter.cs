﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BulkWriter;
using Dapper;
using Dependency.Core;
using Dependency.Core.Models;
using Project = Dependency.Writer.Models.Project;
using ProjectDependency = Dependency.Writer.Models.ProjectDependency;

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

        public void Write(RunResult runResult)
        {
            var projectMap = WriteProjects(runResult);
            WriteProjectDependencies(runResult, projectMap);
        }

        private IDictionary<string, int> WriteProjects(RunResult runResult)
        {
            var projectIntCounter = new IntCounter(-1);
            var projects = runResult
                .Projects
                .Select(x => new Project
                {
                    Id = projectIntCounter.Next(),
                    Name = x.Name
                })
                .ToList();

            using (var bulkWriter = new BulkWriter<Project>(_connectionString))
            {
                bulkWriter.WriteToDatabase(projects);
            }

            return projects.ToDictionary(x => x.Name, x => x.Id);
        }

        private void WriteProjectDependencies(RunResult runResult, IDictionary<string, int> projectMap)
        {
            var projectDependencyIntCounter = new IntCounter(-1);
            var projectDependencies = runResult
                .ProjectDependencies
                .Select(x => new ProjectDependency
                {
                    Id = projectDependencyIntCounter.Next(),
                    ProjectFromId = projectMap[x.From],
                    ProjectToId = projectMap[x.To],
                    Version = x.Version,
                    TargetFramework = x.TargetFramework
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
