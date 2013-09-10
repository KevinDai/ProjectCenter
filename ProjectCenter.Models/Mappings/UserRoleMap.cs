using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class UserRoleMap : EntityTypeConfiguration<UserRole>
    {
        public UserRoleMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("UserRoles");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
