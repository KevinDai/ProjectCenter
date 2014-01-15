using ProjectCenter.Models;
using ProjectCenter.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Services.Models
{
    public class ProjectStatisticItem
    {

        public string UserName
        {
            get;
            private set;
        }

        public ProjectStatusStatistics ManagerStatistics
        {
            get;
            private set;
        }

        public ProjectStatusStatistics ParticipantStatistics
        {
            get;
            private set;
        }

        public int Total
        {
            get
            {
                return ManagerStatistics.Total + ParticipantStatistics.Total;
            }
        }

        public ProjectStatisticItem(User user)
        {
            Preconditions.CheckNotNull(user, "user");

            UserName = user.Name;

            ManagerStatistics = new ProjectStatusStatistics();
            ParticipantStatistics = new ProjectStatusStatistics();
        }

    }

    public class ProjectStatusStatistics
    {
        public int PublishedWaitCheck
        {
            get;
            private set;
        }

        public int PublishedAndChecked
        {
            get;
            private set;
        }

        public int CompletedWaitCheck
        {
            get;
            private set;
        }

        public int CompletedAndChecked
        {
            get;
            private set;
        }

        public int Total
        {
            get
            {
                return PublishedWaitCheck + PublishedAndChecked + CompletedWaitCheck + CompletedAndChecked;
            }
        }

        public void Count(ProjectStatus status)
        {
            switch (status)
            {
                case ProjectStatus.PublishedWaitCheck:
                    PublishedWaitCheck++;
                    break;
                case ProjectStatus.PublishedAndChecked:
                    PublishedAndChecked++;
                    break;
                case ProjectStatus.CompletedWaitCheck:
                    CompletedWaitCheck++;
                    break;
                case ProjectStatus.CompletedAndChecked:
                    CompletedAndChecked++;
                    break;
            }
        }
    }
}
