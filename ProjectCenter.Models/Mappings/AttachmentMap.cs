using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class AttachmentMap : EntityTypeConfiguration<Attachment>
    {
        public AttachmentMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("Attachments");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
