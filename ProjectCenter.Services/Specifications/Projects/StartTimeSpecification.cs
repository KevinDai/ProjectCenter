using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Services.Specifications.Projects
{
    public class StartTimeSpecification : DateTimeSpanSpecification<Project>
    {
        public StartTimeSpecification(DateTime? minTime, DateTime? maxTime)
            : base(p => p.StartTime, minTime, maxTime)
        {
        }
    }
}
