using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class ProjectCategoryMap : EntityTypeConfiguration<ProjectCategory>
    {
        public ProjectCategoryMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("ProjectCategorys");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
