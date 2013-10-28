using ProjectCenter.Util.Query.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ProjectCenter.Services.Specifications
{
    public class DateTimeSpanSpecification<T> : Specification<T>
         where T : class
    {

        protected Expression Property
        {
            get;
            set;
        }

        protected DateTime? MinTime
        {
            get;
            set;
        }

        protected DateTime? MaxTime
        {
            get;
            set;
        }

        public DateTimeSpanSpecification(Expression<Func<T, DateTime>> propety, DateTime? minTime, DateTime? maxTime)
        {
            if (propety == null)
            {
                throw new ArgumentNullException("propety");
            }

            if (!minTime.HasValue && !maxTime.HasValue)
            {
                throw new Exception("至少需要指定最早或者最迟时间");
            }

            Property = propety.Body;
            MinTime = minTime;
            MaxTime = maxTime;
        }

        public override Expression<Func<T, bool>> SatisfiedBy()
        {
            ParameterExpression param = (Property as MemberExpression).Expression as ParameterExpression;
            Expression result = null;

            if (MinTime.HasValue)
            {
                Expression minTimeExp = Expression.Constant(MinTime.Value, typeof(DateTime));
                result = Expression.GreaterThanOrEqual(Property, minTimeExp);
            }

            if (MaxTime.HasValue)
            {
                Expression maxTimeExp = Expression.Constant(MaxTime.Value, typeof(DateTime));
                var lessThan = Expression.LessThan(Property, maxTimeExp);
                result = result == null ? lessThan : Expression.And(result, lessThan);
            }

            if (result == null)
            {
                throw new Exception("至少需要指定最早或者最迟时间");
            }

            return Expression.Lambda<Func<T, bool>>(result, param);
        }
    }
}
