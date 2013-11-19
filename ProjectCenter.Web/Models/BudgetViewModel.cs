using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectCenter.Web.Extensions;
using ProjectCenter.Util;

namespace ProjectCenter.Web.Models
{
    public class BudgetViewModel : Budget
    {
        public BudgetViewModel(Budget budget)
        {
            Preconditions.CheckNotNull(budget, "budget");

            budget.Map(this);

            Expenditures = new List<Expenditure>();
        }

        public List<Expenditure> Expenditures
        {
            get;
            set;
        }
    }
}