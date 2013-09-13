using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    /// <summary>
    /// 项目分类
    /// </summary>
    public enum ProjectType
    {
        /// <summary>
        /// 纵向研究
        /// </summary>
        VerticalResearch = 0,
        
        /// <summary>
        /// 横向研究
        /// </summary>
        HorizontalResearch = 1,

        /// <summary>
        /// 横向咨询
        /// </summary>
        HorizontalEnquire = 2,

        /// <summary>
        /// 纵向工作
        /// </summary>
        VerticalWork = 3,

        /// <summary>
        /// 中心工作
        /// </summary>
        CenterWork = 4

    }
}
