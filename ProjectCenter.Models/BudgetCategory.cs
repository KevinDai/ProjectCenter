using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public enum BudgetCategory
    {
        /// <summary>
        /// 工资（奖金）
        /// </summary>
        SalaryExpenses = 0,

        /// <summary>
        /// 劳务费
        /// </summary>
        ServiceExpenses = 1,

        /// <summary>
        /// 差旅费（交通费）
        /// </summary>
        TravelExpenses = 2,

        /// <summary>
        /// 会议费
        /// </summary>
        MeetingExpenses = 3,

        /// <summary>
        /// 印刷费
        /// </summary>
        PrintingExpenses = 4,

        /// <summary>
        /// 设备购置费
        /// </summary>
        OriginalEquipmentExpenses = 5,

        /// <summary>
        /// 委托业务费
        /// </summary>
        PrincipalBusinessExpenses = 6,

        /// <summary>
        /// 办公费
        /// </summary>
        OfficeExpenses = 7,

        /// <summary>
        /// 管理费
        /// </summary>
        ManagementExpenses = 8,

        /// <summary>
        /// 其他
        /// </summary>
        OtherExpenses = 9
    }
}
