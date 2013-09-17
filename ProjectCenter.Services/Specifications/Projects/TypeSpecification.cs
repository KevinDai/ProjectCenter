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
    public class TypeSpecification : Specification<Project>
    {
        protected int Value
        {
            get;
            set;
        }

        public TypeSpecification(ProjectType type)
        {
            Value = (int)type;
        }

        public TypeSpecification(int type)
        {
            Value = type;
        }

        public override Expression<Func<Project, bool>> SatisfiedBy()
        {
            Expression<Func<Project, bool>> func = p => p.Type == Value;
            return func;
        }
    }
}
