using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class ProjectViewStatusDetail
    {
        public string Id
        {
            get;
            set;
        }

        public string ProjectId
        {
            get;
            set;
        }

        public string UserId
        {
            get;
            set;
        }

        public int ProjectStatus
        {
            get;
            set;
        }

        public int Status
        {
            get;
            set;
        }

        public int FinanceStatus
        {
            get;
            set;
        }

        public string ProjectName
        {
            get;
            set;
        }

        public DateTime UpdateTime
        {
            get;
            set;
        }
    }
}
