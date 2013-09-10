using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class ProjectAttachmentMap : EntityTypeConfiguration<ProjectAttachment>
    {
        public ProjectAttachmentMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("ProjectAttachments");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
