using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectCenter.Models;
using ProjectCenter.Util.Query;

namespace ProjectCenter.Services
{
    public interface IProjectService
    {

        Project GetProject(string id);


        Project AddProject(Project project);

        IEnumerable<Attachment> GetProjectAttachments(string projectId);

        PageList<Comment> GetProjectComments(string projectId);


    }
}
