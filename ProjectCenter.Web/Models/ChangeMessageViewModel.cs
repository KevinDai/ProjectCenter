using ProjectCenter.Models;
using ProjectCenter.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class ChangeMessageViewModel
    {
        public string ProjectId
        {
            get;
            set;
        }

        public string ProjectName
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public bool NeedCheck
        {
            get;
            set;
        }

        public ChangeMessageViewModel(UserInfo userInfo, ProjectViewStatusDetail viewStatus)
        {
            Preconditions.CheckNotNull(userInfo, "userInfo");
            Preconditions.CheckNotNull(viewStatus, "viewStatus");

            ProjectId = viewStatus.ProjectId;
            ProjectName = viewStatus.ProjectName;
            Count = userInfo.RightDetail.EnableManageFinance
                ?
                viewStatus.FinanceStatus + viewStatus.Status
                :
                viewStatus.Status;
            NeedCheck = viewStatus.ProjectStatus == (int)ProjectStatus.PublishedWaitCheck || viewStatus.ProjectStatus == (int)ProjectStatus.CompletedWaitCheck;
        }
    }
}