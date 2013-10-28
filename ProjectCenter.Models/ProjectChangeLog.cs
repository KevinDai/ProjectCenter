using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class ProjectChangeLog
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

        public string UserName
        {
            get;
            set;
        }

        public int ActionType
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }
    }
}
