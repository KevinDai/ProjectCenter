using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Services.Models
{
    public class ExpenditureStatisticItem
    {
        public string ProjectId
        {
            get;
            set;
        }

        public int BudgetCategory
        {
            get;
            set;
        }

        public double Total
        {
            get;
            set;
        }

    }
}
