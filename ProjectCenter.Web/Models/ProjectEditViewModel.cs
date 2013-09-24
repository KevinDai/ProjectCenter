using ProjectCenter.Models;
using ProjectCenter.Util.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectCenter.Web.Extensions;
using ProjectCenter.Util;
using ProjectCenter.Services.Specifications.Projects;

namespace ProjectCenter.Web.Models
{
    public class ProjectEditViewModel : Project
    {
        public ProjectEditViewModel(Project project, UserInfo userInfo)
        {
            Preconditions.CheckNotNull(project, "project");

            project.Map(this);

            var isManager = new ManagerIdsSpecification(userInfo.UserId).SatisfiedBy().Compile()(project);
            EnableSetCompleteCheck = isManager && (project.Status == (int)ProjectStatus.PublishedAndChecked);
            EnableEditProject = userInfo.RightDetail.EnableEditProject
                || isManager
                || project.CreatorId == userInfo.UserId
                || new ParticipantIdsSpecification(userInfo.UserId).SatisfiedBy().Compile()(project);
        }

        public bool EnableEditProject
        {
            get;
            set;
        }

        public bool EnableSetCompleteCheck
        {
            get;
            set;
        }

        public IEnumerable<Attachment> Attachments
        {
            get;
            set;
        }

        public PageList<Comment> CommentPageList
        {
            get;
            set;
        }
    }
}