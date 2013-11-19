using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models.Mappings
{
    internal class BudgetMap : EntityTypeConfiguration<Budget>
    {
        public BudgetMap()
        {
            this.HasKey(ri => ri.Id);

            this.ToTable("Budgets");
        }
    }
}
