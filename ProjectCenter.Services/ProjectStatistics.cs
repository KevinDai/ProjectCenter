using ProjectCenter.Models;
using ProjectCenter.Services.Models;
using ProjectCenter.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Services
{
    internal class ProjectStatistics
    {
        protected IEnumerable<User> Users
        {
            get;
            private set;
        }

        protected IDictionary<string, ProjectStatisticItem> Items
        {
            get;
            private set;
        }

        protected char[] UserIdSplitChar
        {
            get;
            private set;
        }

        public ProjectStatistics(IEnumerable<User> users)
        {
            Preconditions.CheckAny(users, "users", "统计用户信息不能为空");

            UserIdSplitChar = Constants.UserIdPrefix.ToCharArray();
            Users = new List<User>(users);
            Items = new Dictionary<string, ProjectStatisticItem>();
            foreach (var user in users)
            {
                Items.Add(user.Id, new ProjectStatisticItem(user));
            }
        }

        public void Count(IEnumerable<Project> projects)
        {
            if (projects != null && projects.Any())
            {
                ProjectStatisticItem temp;
                foreach (var project in projects)
                {
                    var status = (ProjectStatus)project.Status;
                    Count(project.ManagerIds, status, true);
                    Count(project.ParticipantIds, status, false);
                }
            }
        }

        public IEnumerable<ProjectStatisticItem> GetItems()
        {
            return Items.Values.ToArray();
        }

        private void Count(string userIdsStr, ProjectStatus status, bool countManager)
        {
            ProjectStatisticItem temp;
            if (!string.IsNullOrEmpty(userIdsStr))
            {
                var userIds = userIdsStr.Split(UserIdSplitChar, StringSplitOptions.RemoveEmptyEntries);
                foreach (var userId in userIds)
                {
                    if (Items.TryGetValue(userId, out temp))
                    {
                        if (countManager)
                        {
                            temp.ManagerStatistics.Count(status);
                        }
                        else
                        {
                            temp.ParticipantStatistics.Count(status);
                        }
                    }
                }
            }
        }

    }
}
