using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class ProjectMap : EntityTypeConfiguration<Project>
    {
        public ProjectMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("Projects");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
