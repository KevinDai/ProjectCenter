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

            EnableSetCompleteCheck = userInfo.EnableSetCompleteCheck(project);
            EnableEditProject = userInfo.EnableEditProject(project);
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