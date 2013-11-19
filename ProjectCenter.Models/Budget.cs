using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    /// <summary>
    /// 项目预算信息
    /// </summary>
    public class Budget
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

        public string CategoryString
        {
            get
            {
                return GetBudgetCategoryString((BudgetCategory)Category);
            }
        }

        public static string GetBudgetCategoryString(BudgetCategory category)
        {
            switch (category)
            {
                case BudgetCategory.SalaryExpenses:
                    return "工资（奖金）";
                case BudgetCategory.ServiceExpenses:
                    return "劳务费";
                case BudgetCategory.TravelExpenses:
                    return "差旅费（交通费）";
                case BudgetCategory.MeetingExpenses:
                    return "会议费";
                case BudgetCategory.PrintingExpenses:
                    return "印刷费";
                case BudgetCategory.OriginalEquipmentExpenses:
                    return "设备购置费";
                case BudgetCategory.PrincipalBusinessExpenses:
                    return "委托业务费";
                case BudgetCategory.OfficeExpenses:
                    return "办公费";
                case BudgetCategory.ManagementExpenses:
                    return "管理费";
                case BudgetCategory.OtherExpenses:
                    return "其他";
                default:
                    return "未知预算类型";
            }
        }
    }
}
