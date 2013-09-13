using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    /// <summary>
    /// 项目状态
    /// </summary>
    public enum ProjectStatus
    {
        /// <summary>
        /// 发布待审
        /// </summary>
        PublishedWaitCheck = 0,

        /// <summary>
        /// 发布已审核
        /// </summary>
        PublishedAndChecked = 1,

        /// <summary>
        /// 完成待审
        /// </summary>
        CompletedWaitCheck = 2,

        /// <summary>
        /// 已完成
        /// </summary>
        CompletedAndChecked = 3,

        /// <summary>
        /// 已取消
        /// </summary>
        Canceled = 4
    }
}
