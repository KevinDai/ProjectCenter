using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public enum AmountReceivedStatus
    {
        /// <summary>
        /// 未到款
        /// </summary>
        None = 0,

        /// <summary>
        /// 已收前期款
        /// </summary>
        Part = 1,

        /// <summary>
        /// 已全部到款    
        /// </summary>
        All = 2
    }
}
