﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dependency.Writer.Models
{
    [Table("project")]
    public class Project
    {
        [Key]
        [Column("projectid")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
