using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using ProjectCenter.Util.Query.Specification;
using System.Reflection;

namespace ProjectCenter.Util.Query.Extensions
{
    public static class IQueryableExtension
    {
        private static MethodInfo OrderByMethod = typeof(IQueryableExtension).GetMethod("OrderBy", BindingFlags.Static | BindingFlags.NonPublic);
        private static MethodInfo OrderByDescendingMethod = typeof(IQueryableExtension).GetMethod("OrderByDescending", BindingFlags.Static | BindingFlags.NonPublic);

        public static IQueryable<T> FindBy<T>(this IQueryable<T> query, ISpecification<T> specification) where T : class
        {
            if (specification != null)
            {
                query = query.Where(specification.SatisfiedBy());
            }
            return query;
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, params SortDescriptor<T>[] sortDescriptors) where T : class
        {
            if (sortDescriptors != null && sortDescriptors.Any())
            {
                //反转排序说明对象列表
                var list = sortDescriptors.Reverse();
                foreach (var item in list)
                {
                    var method = item.SortDirection == ListSortDirection.Ascending ? OrderByMethod : OrderByDescendingMethod;
                    query = method.MakeGenericMethod(new Type[] { typeof(T), item.KeyType })
                        .Invoke(null, new object[] { query, item.SortKeySelector }) as IQueryable<T>;
                    //?
                    //OrderByMethod.MakeGenericMethod(new Type[] { typeof(T), item.KeyType })
                    //.Invoke(null, new object[] { query, item.SortKeySelector }) as IQueryable<T>
                    //typeof(IQueryable<T>).GetMethod("OrderBy").Invoke(query, new object[] { item.SortKeySelector }) as IQueryable<T> 
                    //query.OrderBy(item.SortKeySelector)
                    //:
                    //typeof(System.Linq.Queryable).GetMethod("OrderByDescending")
                    //.Invoke(null, new object[] { query, item.SortKeySelector }) as IQueryable<T>;
                    //typeof(IQueryable<T>).GetMethod("OrderByDescending").Invoke(query, new object[] { item.SortKeySelector }) as IQueryable<T>;
                    //query.OrderByDescending(item.SortKeySelector);
                }
            }
            //else
            //{
            //query = query.OrderBy(e => e.Id);
            //}
            return query;
        }

        private static IQueryable<T> OrderBy<T, TValue>(IQueryable<T> source, Expression<Func<T, TValue>> keySelector)
        {
            return System.Linq.Queryable.OrderBy(source, keySelector);
        }

        private static IQueryable<T> OrderByDescending<T, TValue>(IQueryable<T> source, Expression<Func<T, TValue>> keySelector)
        {
            return System.Linq.Queryable.OrderByDescending(source, keySelector);
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            //if (pageIndex <= 0)
            //{
            //    throw new ArgumentException(
            //        "分页页码必须大于零",
            //        "pageIndex");
            //}
            //if (pageSize <= 0)
            //{
            //    throw new ArgumentException(
            //        "分页大小必须大于零",
            //        "pageCount");
            //}
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return query;
        }

        public static PageList<T> PageList<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            var total = query.Count();
            if (total > 0)
            {
                var maxPageCount =
                    total % pageSize == 0
                    ?
                    total / pageSize
                    :
                    total / pageSize + 1;

                if (pageIndex <= 0)
                {
                    pageIndex = 1;
                }
                else if (pageIndex > maxPageCount)
                {
                    pageIndex = maxPageCount;
                }

                var list = query.Paginate(pageIndex, pageSize).ToArray();

                return new PageList<T>(list, pageIndex, pageSize, total);
            }
            else
            {
                return new PageList<T>(new T[0], 1, pageSize, total);
            }
        }
    }
}
