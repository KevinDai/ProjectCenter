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

            EnableSave = userInfo.RightDetail.EnableEditProject
                || project.CreatorId == userInfo.UserId
                || new ManagerIdsSpecification(userInfo.UserId).SatisfiedBy().Compile()(project)
                || new ParticipantIdsSpecification(userInfo.UserId).SatisfiedBy().Compile()(project);
        }

        public bool EnableSave
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