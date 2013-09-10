using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace ProjectCenter.Models.Mappings
{
    internal class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("Users");

            //this.Property(ri => ri.Id)
            //    .HasColumnName("Id");
        }
    }
}
