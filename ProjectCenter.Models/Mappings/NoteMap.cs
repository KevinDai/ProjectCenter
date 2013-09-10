using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class NoteMap : EntityTypeConfiguration<Note>
    {
        public NoteMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("Notes");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
