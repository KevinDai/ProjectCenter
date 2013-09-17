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
    public class NameSpecification : Specification<Project>
    {
        protected string Value
        {
            get;
            set;
        }

        public NameSpecification(string value)
        {
            Preconditions.CheckNotBlank(value, "value");

            Value = value;
        }

        public override Expression<Func<Project, bool>> SatisfiedBy()
        {
            Expression<Func<Project, bool>> func = p => p.Name.Contains(Value);
            return func;
        }
    }
}
