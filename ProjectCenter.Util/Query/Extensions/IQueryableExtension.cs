using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using ProjectCenter.Util.Query.Specification;

namespace ProjectCenter.Util.Query.Extensions
{
    public static class IQueryableExtension
    {
        internal static IQueryable<T> FindBy<T, TId>(this IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification != null)
            {
                query = query.Where(specification.SatisfiedBy());
            }
            return query;
        }

        internal static IQueryable<T> Sort<T, TId>(this IQueryable<T> query, params SortDescriptor<T>[] sortDescriptors) where T : class
        {
            if (sortDescriptors != null && sortDescriptors.Any())
            {
                //反转排序说明对象列表
                var list = sortDescriptors.Reverse();
                foreach (var item in list)
                {
                    query = item.SortDirection == ListSortDirection.Ascending
                        ?
                        query.OrderBy(item.SortKeySelector)
                        :
                        query.OrderByDescending(item.SortKeySelector);
                }
            }
            //else
            //{
            //query = query.OrderBy(e => e.Id);
            //}
            return query;
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
            {
                throw new ArgumentException(
                    "分页页码必须大于零",
                    "pageIndex");
            }
            if (pageSize <= 0)
            {
                throw new ArgumentException(
                    "分页大小必须大于零",
                    "pageCount");
            }
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return query;
        }

    }
}
