using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectCenter.Util;
using ProjectCenter.Util.Query.Specification;
using ProjectCenter.Models;
using System.Linq.Expressions;

namespace ProjectCenter.Services.Specifications.Projects
{
    public class DeadlineSpecification : Specification<Project>
    {
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

        public DeadlineSpecification(DateTime? minTime, DateTime? maxTime)
        {
            if (!minTime.HasValue && maxTime.HasValue)
            {
                throw new Exception("至少需要指定最早或者最迟时间");
            }

            MinTime = minTime;
            MaxTime = maxTime;
        }

        public override Expression<Func<Project, bool>> SatisfiedBy()
        {
            if (MinTime.HasValue && MaxTime.HasValue)
            {
                return q => q.Deadline >= MinTime.Value && q.Deadline < MaxTime;
            }
            else if (MinTime.HasValue)
            {
                return q => q.Deadline >= MinTime.Value;
            }
            else if (MaxTime.HasValue)
            {
                return q => q.Deadline < MaxTime.Value;
            }
            else
            {
                throw new Exception("至少需要指定最早或者最迟时间");
            }
        }
    }
}
