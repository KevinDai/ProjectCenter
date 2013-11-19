using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class Expenditure
    {
        public string Id
        {
            get;
            set;
        }

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

        public double Count
        {
            get;
            set;
        }

        public string Remark
        {
            get;
            set;
        }

        public string CreatorId
        {
            get;
            set;
        }

        public string CreatorName
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public string BudgetCategoryString
        {
            get
            {
                return Budget.GetBudgetCategoryString((BudgetCategory)BudgetCategory);
            }
        }
    }
}
