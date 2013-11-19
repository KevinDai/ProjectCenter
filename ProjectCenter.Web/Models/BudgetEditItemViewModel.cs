using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class BudgetEditItemViewModel
    {
        public int Category
        {
            get;
            set;
        }

        public double Amount
        {
            get;
            set;
        }
    }
}