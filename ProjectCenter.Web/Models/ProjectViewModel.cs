using ProjectCenter.Models;
using ProjectCenter.Util.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectCenter.Web.Extensions;
using ProjectCenter.Util;

namespace ProjectCenter.Web.Models
{
    public class ProjectViewModel : Project
    {
        public ProjectViewModel(Project project)
        {
            Preconditions.CheckNotNull(project, "project");

            project.Map(this);
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