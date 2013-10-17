using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using ProjectCenter.Models;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Extensions;
using System.Transactions;
using ProjectCenter.Util.Query.Specification;

namespace ProjectCenter.Services.Imp
{
    internal class ProjectService : ServiceBase, IProjectService
    {

        protected IDbSet<Project> Projects
        {
            get
            {
                return DbContext.Set<Project>();
            }
        }

        protected IDbSet<Attachment> Attachments
        {
            get
            {
                return DbContext.Set<Attachment>();
            }
        }

        protected IDbSet<Comment> Comments
        {
            get
            {
                return DbContext.Set<Comment>();
            }
        }

        public ProjectService(DbContext dbContext)
            : base(dbContext)
        {
        }


        #region 方法

        protected IEnumerable<Comment> GetProjectComments(string projectId)
        {
            return Comments.Where(c => c.ProjectId == projectId).ToArray();
        }

        private void ProjectValidSet(Project project)
        {
            project.ManagerIds = project.ManagerIds ?? string.Empty;
            project.ParticipantIds = project.ParticipantIds ?? string.Empty;
            var amountReceivedStatus = (AmountReceivedStatus)project.AmountReceivedStatus;
            switch (amountReceivedStatus)
            {
                case AmountReceivedStatus.None:
                    project.AmountReceived = 0;
                    break;
                case AmountReceivedStatus.All:
                    project.AmountReceived = project.Amount;
                    break;
            }
        }

        #endregion

        #region IProjectService接口实现

        public PageList<Project> GetProjectPageList(ISpecification<Project> spec, SortDescriptor<Project>[] sorts, int pageIndex, int pageSize)
        {
            var query = Projects as IQueryable<Project>;
            if (spec != null)
            {
                query = query.Where(spec.SatisfiedBy());
            }
            if (sorts != null)
            {
                query = query.Sort(sorts);
            }

            return query.PageList(pageIndex, pageSize);
        }

        public Project GetProject(string projectId)
        {
            return Projects.Find(projectId);
        }

        public Project AddProject(Project project)
        {
            project.Id = Guid.NewGuid().ToString();
            project.CreateTime = DateTime.Now;

            ProjectValidSet(project);

            AddEntity(project);

            //if (attachmentIds != null && attachmentIds.Any())
            //{
            //    var attachments = Attachments.Where(q => attachmentIds.Contains(q.Id)).ToArray();
            //    foreach (var item in attachments)
            //    {
            //        item.ProjectId = project.Id;
            //    }
            //}
            SaveChanges();
            return project;
        }

        public IEnumerable<Attachment> GetProjectAttachments(string projectId)
        {
            return Attachments.Where(q => q.ProjectId == projectId).OrderBy(q => q.UploadTime).ToArray();
        }

        public PageList<Comment> GetProjectCommentPageList(string projectId, int pageIndex, int pageSize)
        {
            return Comments.Where(c => c.ProjectId == projectId)
                .OrderByDescending(q => q.CreateTime)
                .PageList(pageIndex, pageSize);
        }

        public void CheckProjects(string[] projectIds, ProjectStatus status)
        {
            if (projectIds != null && projectIds.Any())
            {
                var projects = Projects.Where(p => projectIds.Contains(p.Id)).ToArray();
                foreach (var item in projects)
                {
                    item.Status = (int)status;
                    UpdateEntity(item);
                }
                SaveChanges();
            }
        }

        public Project UpdateProject(Project project)
        {
            ProjectValidSet(project);

            UpdateEntity(project);

            SaveChanges();

            return project;
        }

        public void DeleteProject(Project project)
        {
            DeleteEntity(project);

            var attachments = GetProjectAttachments(project.Id);
            foreach (var item in attachments)
            {
                DeleteEntity(item);
            }

            var comments = GetProjectComments(project.Id);
            foreach (var item in comments)
            {
                DeleteEntity(item);
            }

            SaveChanges();
        }

        public Attachment AddAttachment(Attachment attachment)
        {
            attachment.Id = Guid.NewGuid().ToString();
            attachment.UploadTime = DateTime.Now;

            AddEntity(attachment);

            SaveChanges();

            return attachment;
        }

        public Attachment GetAttachment(string attachmentId)
        {
            return Attachments.Find(attachmentId);
        }

        public IEnumerable<Attachment> GetAttachments(string[] attachmentIds)
        {
            return Attachments.Where(q => attachmentIds.Contains(q.Id)).ToArray();
        }


        public void DeleteAttachment(Attachment attachment)
        {
            DeleteEntity(attachment);

            SaveChanges();
        }

        public Comment AddComment(Comment comment)
        {
            comment.Id = Guid.NewGuid().ToString();
            comment.CreateTime = DateTime.Now;

            AddEntity(comment);

            SaveChanges();

            return comment;
        }

        public Comment GetComment(string commentId)
        {
            return Comments.Find(commentId);
        }

        public void DeleteComment(Comment comment)
        {
            DeleteEntity(comment);

            SaveChanges();
        }

        #endregion

    }
}
