using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models.Mappings
{
    internal class ExpenditureMap : EntityTypeConfiguration<Expenditure>
    {
        public ExpenditureMap()
        {
            this.HasKey(ri => ri.Id);

            this.Property(e => e.Count).IsRequired();
            this.ToTable("Expenditures");
        }
    }
}
