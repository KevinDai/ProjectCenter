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
        public int Type
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
        /// 项目状态
        /// </summary>
        public int Status
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
        /// 需要支持
        /// </summary>
        public string NeedSupport
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
        /// 收款状态
        /// </summary>
        public int AmountReceivedStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 已收金额
        /// </summary>
        public double AmountReceived
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
        /// 负责人Id
        /// </summary>
        public string ManagerIds
        {
            get;
            set;
        }

        /// <summary>
        /// 负责人名称
        /// </summary>
        public string ManagerNames
        {
            get;
            set;
        }

        /// <summary>
        /// 参与者Id
        /// </summary>
        public string ParticipantIds
        {
            get;
            set;
        }

        /// <summary>
        /// 参与者名称
        /// </summary>
        public string ParticipantNames
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public string CreatorId
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName
        {
            get;
            set;
        }

        /// <summary>
        /// 最新动态
        /// </summary>
        public string LatestNews
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

        public string TypeString
        {
            get
            {
                switch ((ProjectType)Type)
                {
                    case ProjectType.VerticalResearch:
                        return "纵向研究";
                    case ProjectType.HorizontalResearch:
                        return "横向研究";
                    case ProjectType.HorizontalEnquire:
                        return "横向咨询";
                    case ProjectType.VerticalWork:
                        return "纵向工作";
                    case ProjectType.CenterWork:
                        return "中心工作";
                    default:
                        return "未归类";
                }
            }
        }

        public string StatusString
        {
            get
            {
                switch ((ProjectStatus)Status)
                {
                    case ProjectStatus.PublishedWaitCheck:
                        return "发布待审";
                    case ProjectStatus.PublishedAndChecked:
                        return "进行中";
                    case ProjectStatus.CompletedWaitCheck:
                        return "完成待审";
                    case ProjectStatus.CompletedAndChecked:
                        return "已完成";
                    default:
                        return "未知状态";
                }
            }
        }
    }
}
