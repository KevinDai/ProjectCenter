using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class QueryFilter
    {
        public FieldFilter[] FieldFilters
        {
            get;
            set;
        }

        public SortField[] SortFields
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }
    }

    public class FieldFilter
    {
        public string Field
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }

    public class SortField
    {
        public string Field
        {
            get;
            set;
        }

        public bool IsAsc
        {
            get;
            set;
        }
    }
}