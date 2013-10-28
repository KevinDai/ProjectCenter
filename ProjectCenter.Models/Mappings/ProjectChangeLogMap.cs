using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models.Mappings
{
    public class ProjectChangeLogMap : EntityTypeConfiguration<ProjectChangeLog>
    {
        public ProjectChangeLogMap()
        {
            this.HasKey(pcl => pcl.Id);

            this.ToTable(" ProjectChangeLogs");
        }
    }
}
