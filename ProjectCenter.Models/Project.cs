using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class Project
    {
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 任务类型Id
        /// </summary>
        public string CategoryId
        {
            get;
            set;
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 委托单位
        /// </summary>
        public string Consignor
        {
            get;
            set;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime Deadline
        {
            get;
            set;
        }

        /// <summary>
        /// 需完成材料
        /// </summary>
        public string NeedFinish
        {
            get;
            set;
        }

        /// <summary>
        /// 当前进展
        /// </summary>
        public string CurrentProgress
        {
            get;
            set;
        }

        /// <summary>
        /// 推进计划
        /// </summary>
        public string AdvancePlan
        {
            get;
            set;
        }

        /// <summary>
        /// 项目金额
        /// </summary>
        public double Amount
        {
            get;
            set;
        }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string TypeOfPayment
        {
            get;
            set;
        }

        /// <summary>
        /// 已收金额
        /// </summary>
        public string AmountReceived
        {
            get;
            set;
        }

        /// <summary>
        /// 项目状态
        /// </summary>
        public int Status
        {
            get;
            set;
        }

        /// <summary>
        /// 发票状态
        /// </summary>
        public int InvoiceStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 任务类型
        /// </summary>
        public ProjectCategory Category
        {
            get;
            set;
        }

    }
}
