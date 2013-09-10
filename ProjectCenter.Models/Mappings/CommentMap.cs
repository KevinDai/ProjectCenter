using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class CommentMap : EntityTypeConfiguration<Comment>
    {
        public CommentMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("Comments");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
