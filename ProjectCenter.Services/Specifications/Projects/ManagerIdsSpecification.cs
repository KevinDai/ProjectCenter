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
    public class ManagerIdsSpecification : Specification<Project>
    {
        protected string Value
        {
            get;
            set;
        }

        public ManagerIdsSpecification(string userId)
        {
            Preconditions.CheckNotBlank(userId, "userId");

            Value = Constants.UserIdPrefix + userId;
        }

        public override Expression<Func<Project, bool>> SatisfiedBy()
        {
            Expression<Func<Project, bool>> func = p => p.ManagerIds.Contains(Value);
            return func;
        }
    }
}
