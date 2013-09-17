using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace ProjectCenter.Util.Query
{
    public class SortDescriptor<T> where T : class
    {
        #region Members

        //public Expression SortKeySelector
        //{
        //    get;
        //    private set;
        //}

        public string Field
        {
            get;
            private set;
        }

        public ListSortDirection SortDirection
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// 构造函数，默认为升序排列
        /// </summary>
        /// <param name="field"></param>
        public SortDescriptor(string field)
            : this(field, ListSortDirection.Ascending)
        {
        }

        public SortDescriptor(string field, ListSortDirection sortDirection)
        {
            Field = field;
            SortDirection = sortDirection;
        }

        #endregion
    }
}
