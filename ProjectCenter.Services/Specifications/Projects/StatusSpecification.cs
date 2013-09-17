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
    public class StatusSpecification : Specification<Project>
    {
        protected int Value
        {
            get;
            set;
        }

        public StatusSpecification(ProjectStatus status)
        {
            Value = (int)status;
        }

        public StatusSpecification(int status)
        {
            Value = status;
        }

        public override Expression<Func<Project, bool>> SatisfiedBy()
        {
            Expression<Func<Project, bool>> func = p => p.Status == Value;
            return func;
        }
    }
}
