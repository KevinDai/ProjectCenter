using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using ProjectCenter.Models;
using ProjectCenter.Util.Query;

namespace ProjectCenter.Services.Imp
{
    internal class ProjectService : ServiceBase, IProjectService
    {
        public ProjectService(DbContext dbContext)
            : base(dbContext)
        {
        }



        public Project GetProject(string id)
        {
            throw new NotImplementedException();
        }

        public Project AddProject(Project project)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Attachment> GetProjectAttachments(string projectId)
        {
            throw new NotImplementedException();
        }

        public PageList<Comment> GetProjectComments(string projectId)
        {
            throw new NotImplementedException();
        }
    }
}
