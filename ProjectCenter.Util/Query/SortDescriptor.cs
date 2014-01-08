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

        public Type KeyType
        {
            get;
            private set;
        }

        public Expression SortKeySelector
        {
            get;
            private set;
        }

        //public string Field
        //{
        //    get;
        //    private set;
        //}

        public ListSortDirection SortDirection
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        ///// <summary>
        ///// 构造函数，默认为升序排列
        ///// </summary>
        ///// <param name="field"></param>
        //public SortDescriptor(string field)
        //    : this(field, ListSortDirection.Ascending)
        //{
        //}

        //public SortDescriptor(string field, ListSortDirection sortDirection)
        //{
        //    Field = field;
        //    SortDirection = sortDirection;
        //}

        private SortDescriptor()
        {
        }

        //public SortDescriptor(Expression<Func<T, TValue>> exp)
        //{
        //}

        #endregion

        public static SortDescriptor<T> CreateSortDescriptor<TValue>(Expression<Func<T, TValue>> exp, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var sort = new SortDescriptor<T>();
            sort.KeyType = typeof(TValue);
            sort.SortKeySelector = exp;
            sort.SortDirection = sortDirection;
            return sort;
        }
    }
}
