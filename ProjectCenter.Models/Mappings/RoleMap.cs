using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("Roles");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
