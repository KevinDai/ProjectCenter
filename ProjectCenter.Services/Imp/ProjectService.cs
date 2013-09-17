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
                for (int i = sorts.Length - 1; i >= 0; i--)
                {
                    var sort = sorts[i];
                    switch (sort.Field)
                    {
                        case "Deadline":
                            if (sort.SortDirection == ListSortDirection.Ascending)
                            {
                                query = query.OrderBy(q => q.Deadline);
                            }
                            else
                            {
                                query = query.OrderByDescending(q => q.Deadline);
                            }
                            break;
                    }
                }
                //query = query.Sort(sort);
            }

            return query.PageList(pageIndex, pageSize);
        }

        public Project GetProject(string projectId)
        {
            return Projects.Find(projectId);
        }

        public Project AddProject(Project project, string[] attachmentIds)
        {
            project.Id = Guid.NewGuid().ToString();
            project.CreateTime = DateTime.Now;

            AddEntity(project);

            if (attachmentIds != null && attachmentIds.Any())
            {
                var attachments = Attachments.Where(q => attachmentIds.Contains(q.Id)).ToArray();
                foreach (var item in attachments)
                {
                    item.ProjectId = project.Id;
                }
            }
            SaveChanges();
            return project;
        }

        public IEnumerable<Attachment> GetProjectAttachments(string projectId)
        {
            return Attachments.Where(q => q.ProjectId == projectId).ToArray();
        }

        public PageList<Comment> GetProjectCommentPageList(string projectId, int pageIndex, int pageSize)
        {
            return Comments.Where(c => c.ProjectId == projectId)
                .OrderByDescending(q => q.CreateTime)
                .PageList(pageIndex, pageSize);
        }

        public Project UpdateProject(Project project)
        {
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

            AddAttachment(attachment);

            SaveChanges();

            return attachment;
        }

        public Attachment GetAttachment(string attachmentId)
        {
            return Attachments.Find(attachmentId);
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
