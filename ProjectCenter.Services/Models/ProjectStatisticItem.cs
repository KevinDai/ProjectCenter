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

        public ProjectStatusStatistics ManagerCount
        {
            get;
            private set;
        }

        public ProjectStatusStatistics ParticipantCount
        {
            get;
            private set;
        }


        public ProjectStatisticItem(User user)
        {
            Preconditions.CheckNotNull(user, "user");

            UserName = user.Name;

            ManagerCount = new ProjectStatusStatistics();
            ParticipantCount = new ProjectStatusStatistics();
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
