using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    /// <summary>
    /// 开票状态
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// 未开票
        /// </summary>
        None = 0,

        /// <summary>
        /// 已开票
        /// </summary>
        Release = 1,

        /// <summary>
        /// 已开到账证明
        /// </summary>
        AccountCertificate = 2
    }
}
