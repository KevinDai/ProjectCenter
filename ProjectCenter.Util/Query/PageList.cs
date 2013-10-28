using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Util.Query
{
    public class PageList<T>
    {
        #region Members

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 对象总数
        /// </summary>
        public int Total
        {
            get;
            private set;
        }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize
        {
            get;
            private set;
        }

        ///// <summary>
        ///// 分页页数
        ///// </summary>
        //public int PageCount
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 当前页对象列表
        /// </summary>
        public IEnumerable<T> List
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        public PageList(IEnumerable<T> list, int pageIndex, int pageSize, int totalCount)
        {
            List = list;
            PageSize = pageSize;
            Total = totalCount;
            PageIndex = pageIndex;
        }

        #endregion

    }
}
