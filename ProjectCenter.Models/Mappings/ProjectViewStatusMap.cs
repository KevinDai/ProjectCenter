using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models.Mappings
{
    internal class ProjectViewStatusMap : EntityTypeConfiguration<ProjectViewStatus>
    {
        public ProjectViewStatusMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("ProjectViewStatuses");

        }
    }
}
