using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class ProjectUser
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

        public string IsManager
        {
            get;
            set;
        }

    }
}
