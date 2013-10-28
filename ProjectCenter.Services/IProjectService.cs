﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectCenter.Models;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;

namespace ProjectCenter.Services
{
    public interface IProjectService
    {
        PageList<Project> GetProjectPageList(ISpecification<Project> specs, SortDescriptor<Project>[] sorts, int pageIndex, int pageSize);

        Project GetProject(string projectId);

        IEnumerable<Attachment> GetProjectAttachments(string projectId);

        PageList<Comment> GetProjectCommentPageList(string projectId, int pageIndex, int pageSize);

        Project AddProject(Project project);

        void CheckProjects(string[] projectIds, ProjectStatus status);

        Project UpdateProject(Project project);

        void DeleteProject(Project project);

        Attachment AddAttachment(Attachment attachment);

        Attachment GetAttachment(string attachmentId);

        IEnumerable<Attachment> GetAttachments(string[] attachmentIds);

        void DeleteAttachment(Attachment attachment);

        Comment AddComment(Comment comment);

        Comment GetComment(string commentId);

        void DeleteComment(Comment comment);

        ProjectChangeLog AddChangeLog(ProjectChangeLog log);
    }
}
