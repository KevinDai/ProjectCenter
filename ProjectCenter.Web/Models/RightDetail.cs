using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class RightDetail
    {

        public RightDetail(int rightLevel)
        {
            EnableViewList = rightLevel > 0;
            EnableViewDetail = rightLevel > 0;
            EnableEditProject = rightLevel == 1;
            EnableUserManage = rightLevel == 1;
            EnableCheckProject = rightLevel == 1;
            EnableSetProjectUser = rightLevel == 1;
            EnbaleDeleteProject = rightLevel == 1;
            EnableManageFinance = rightLevel == 1 || rightLevel == 4;
        }

        public bool EnableViewList
        {
            get;
            set;
        }

        public bool EnableViewDetail
        {
            get;
            set;
        }

        public bool EnableCheckProject
        {
            get;
            set;
        }

        public bool EnableEditProject
        {
            get;
            set;
        }

        public bool EnableSetProjectUser
        {
            get;
            set;
        }

        public bool EnbaleDeleteProject
        {
            get;
            set;
        }

        public bool EnableManageFinance
        {
            get;
            set;
        }

        public bool EnableUserManage
        {
            get;
            set;
        }
    }
}