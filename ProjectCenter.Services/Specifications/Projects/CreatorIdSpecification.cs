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
    public class CreatorIdSpecification : Specification<Project>
    {
        protected string Value
        {
            get;
            set;
        }

        public CreatorIdSpecification(string userId)
        {
            Preconditions.CheckNotBlank(userId, "userId");

            Value = userId;
        }

        public override Expression<Func<Project, bool>> SatisfiedBy()
        {
            Expression<Func<Project, bool>> func = p => p.CreatorId == Value;
            return func;
        }
    }
}
