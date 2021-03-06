﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DependencyTracker.MssqlWriter
{
    [Table("projectdependency")]
    public class ProjectDependency
    {
        [Key]
        [Column("projectdependencyid")]
        public int Id { get; set; }

        [Column("projectfromid")]
        public int ProjectFromId { get; set; }

        [Column("projecttoid")]
        public int ProjectToId { get; set; }

        [Column("version")]
        public string Version { get; set; }

        [Column("targetframework")]
        public string TargetFramework { get; set; }

        [Column("type")]
        public string Type { get; set; }
    }
}
