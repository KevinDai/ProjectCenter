﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using ProjectCenter.Models;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Extensions;
using System.Transactions;
using ProjectCenter.Util.Query.Specification;
using System.Data.SqlClient;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Services.Models;

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

        protected IDbSet<ProjectViewStatus> ProjectViewStatuses
        {
            get
            {
                return DbContext.Set<ProjectViewStatus>();
            }
        }

        protected IDbSet<Attachment> Attachments
        {
            get
            {
                return DbContext.Set<Attachment>();
            }
        }

        protected IDbSet<Budget> Budgets
        {
            get
            {
                return DbContext.Set<Budget>();
            }
        }


        protected IDbSet<Expenditure> Expenditures
        {
            get
            {
                return DbContext.Set<Expenditure>();
            }
        }

        protected IDbSet<Comment> Comments
        {
            get
            {
                return DbContext.Set<Comment>();
            }
        }


        protected IDbSet<ProjectChangeLog> ProjectChangeLogs
        {
            get
            {
                return DbContext.Set<ProjectChangeLog>();
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

        private void UpdateLatestNews(ProjectChangeLog log)
        {
            ProjectActionType actionType = (ProjectActionType)log.ActionType;
            if (actionType != ProjectActionType.Delete)
            {
                var latestNews = log.CreateTime.ToString("yyyy-MM-dd HH:mm") + " " + log.UserName + log.ActionTypeString;
                latestNews = string.IsNullOrWhiteSpace(log.Message) ? latestNews : latestNews + "," + log.Message;

                switch (actionType)
                {
                    case ProjectActionType.EditBudget:
                    case ProjectActionType.AddExpenditure:
                    case ProjectActionType.DeleteExpenditure:
                        DbContext.Database.ExecuteSqlCommand("Update Projects Set FinanceLatestNews = @p0 Where Id = @p1",
                            new SqlParameter { ParameterName = "p0", Value = latestNews },
                            new SqlParameter { ParameterName = "p1", Value = log.ProjectId });
                        DbContext.Database.ExecuteSqlCommand("Update ProjectViewStatuses Set FinanceStatus = FinanceStatus + 1, UpdateTime = GETDATE() Where ProjectId = @p0 And UserId <> @p1",
                            new SqlParameter { ParameterName = "p0", Value = log.ProjectId },
                            new SqlParameter { ParameterName = "p1", Value = log.UserId });
                        break;
                    default:
                        DbContext.Database.ExecuteSqlCommand("Update Projects Set LatestNews = @p0, FinanceLatestNews = '' Where Id = @p1",
                            new SqlParameter { ParameterName = "p0", Value = latestNews },
                            new SqlParameter { ParameterName = "p1", Value = log.ProjectId });

                        DbContext.Database.ExecuteSqlCommand("Update ProjectViewStatuses Set Status = Status + 1, UpdateTime = GETDATE() Where ProjectId = @p0 And UserId <> @p1",
                            new SqlParameter { ParameterName = "p0", Value = log.ProjectId },
                            new SqlParameter { ParameterName = "p1", Value = log.UserId });
                        break;
                }

            }
        }

        private double GetExpendituresTotalCount(string projectId, int budgetCategory)
        {
            double? result = Expenditures.Where(q => q.ProjectId == projectId && q.BudgetCategory == budgetCategory).Select(q => (double?)q.Count).Sum();
            return result ?? 0.0;
        }


        private void SetProjectCode(Project project)
        {
            var codePrefix = string.Format("{0}{1}-", GetTypePrefix((ProjectType)project.Type), project.CreateTime.Year.ToString());
            var maxCode = Projects.Where(q => q.Code.StartsWith(codePrefix)).OrderByDescending(q => q.Code).Select(q => q.Code).FirstOrDefault();
            var index = 1;
            if (!string.IsNullOrEmpty(maxCode))
            {
                index = int.Parse(maxCode.Substring(9, 3)) + 1;
            }
            if (index > 999)
            {
                throw new Exception("今天该类型项目的编码已经达到最大值，请联系管理员");
            }
            project.Code = codePrefix + string.Format("{0:D3}", index.ToString("D3"));
        }

        private string GetTypePrefix(ProjectType type)
        {
            switch (type)
            {
                case ProjectType.VerticalResearch:
                    return "ZXYJ";
                case ProjectType.HorizontalResearch:
                    return "HXYJ";
                case ProjectType.HorizontalEnquire:
                    return "HXZX";
                case ProjectType.VerticalWork:
                    return "ZXGZ";
                case ProjectType.CenterWork:
                    return "BMGZ";
                default:
                    throw new Exception("异常的项目类型");
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

        /// <summary>
        /// 获取项目的支出统计信息
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public IEnumerable<ExpenditureStatisticItem> GetExpenditureStatistics(ISpecification<Project> spec, int pageIndex, int pageSize)
        {
            var projectsQuery = Projects as IQueryable<Project>;
            if (spec != null)
            {
                projectsQuery = projectsQuery.Where(spec.SatisfiedBy());
            }
            projectsQuery = projectsQuery.OrderBy(p => p.Id);
            projectsQuery = projectsQuery.Paginate(pageIndex, pageSize);

            var expendituresQuery = Expenditures as IQueryable<Expenditure>;

            expendituresQuery = expendituresQuery.Where(e => projectsQuery.Any(p => p.Id == e.ProjectId));
            var result = expendituresQuery
                 .GroupBy(q => new
                 {
                     ProjectId = q.ProjectId,
                     BudgetCategory = q.BudgetCategory
                 })
                 .Select(q => new ExpenditureStatisticItem()
                 {
                     ProjectId = q.Key.ProjectId,
                     BudgetCategory = q.Key.BudgetCategory,
                     Total = q.Sum(t => t.Count)
                 }).ToArray();
            return result;
        }

        public IEnumerable<ProjectStatisticItem> GetProjectStatistics(ISpecification<Project> spec)
        {
            //控制每次处理数据量
            var pageSize = 1000;

            var users = Users.OrderBy(u => u.RightLevel).ToArray();
            var projectStatistics = new ProjectStatistics(users);

            var query = Projects as IQueryable<Project>;
            if (spec != null)
            {
                query = query.Where(spec.SatisfiedBy());
            }

            var count = query.Count();
            var pageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;

            //增加排序，否则SQL无法分页
            query = query.OrderBy(q => q.Id);

            for (int i = 0; i < pageCount; i++)
            {
                var projects = query.Paginate(i + 1, pageSize).ToArray();
                projectStatistics.Count(projects);
            }

            return projectStatistics.GetItems();
        }

        public Project GetProject(string projectId)
        {
            var project = Projects.Find(projectId);
            return project;
        }

        public void UpdateProjectViewStatusRead(string projectId, string userId)
        {
            var pvs = ProjectViewStatuses.FirstOrDefault(q => q.ProjectId == projectId && q.UserId == userId);
            if (pvs == null)
            {
                pvs = new ProjectViewStatus()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    ProjectId = projectId,
                    Status = 0,
                    FinanceStatus = 0
                };
                ProjectViewStatuses.Add(pvs);
                SaveChanges();
            }
            else
            {
                pvs.Status = 0;
                pvs.FinanceStatus = 0;
                UpdateEntity(pvs);
                SaveChanges();
            }
        }

        public void UpdateAllProjectViewStatusRead(string userId)
        {
            DbContext.Database.ExecuteSqlCommand(
                "Update ProjectViewStatuses Set Status = 0, FinanceStatus = 0, UpdateTime = GETDATE() Where UserId = @p0 And ( Status > 0 Or FinanceStatus > 0 )",
                new SqlParameter { ParameterName = "p0", Value = userId });
        }

        public IEnumerable<ProjectViewStatus> GetProjectViewStatus(string[] projectIds, string userId)
        {
            return ProjectViewStatuses.Where(q => projectIds.Contains(q.ProjectId) && q.UserId == userId).ToArray();
        }

        public Project AddProject(Project project)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                project.Id = Guid.NewGuid().ToString();
                project.CreateTime = DateTime.Now;

                ProjectValidSet(project);
                SetProjectCode(project);
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

                DbContext.Database.ExecuteSqlCommand(
                @"INSERT INTO [ProjectViewStatuses] 
                SELECT NEWID() AS [Id], @p0 AS [ProjectId], U.ID AS [UserId], 0 AS [Status], 0 AS [FinanceStatus], GETDATE() AS [UpdateTime]
                FROM Users U
                WHERE  U.RIGHTLEVEL >= 0",
                    new SqlParameter { ParameterName = "p0", Value = project.Id });

                ts.Complete();
                return project;
            }
        }

        public IEnumerable<Attachment> GetProjectAttachments(string projectId)
        {
            return Attachments.Where(q => q.ProjectId == projectId).OrderBy(q => q.UploadTime).ToArray();
        }

        public IEnumerable<Budget> GetProjectBudgets(string projectId)
        {
            return Budgets.Where(q => q.ProjectId == projectId).OrderBy(q => q.Category).ToArray();
        }

        public IEnumerable<Expenditure> GetProjectExpenditures(string projectId)
        {
            return Expenditures.Where(q => q.ProjectId == projectId).OrderBy(q => q.CreateTime).ToArray();
        }

        public IEnumerable<Expenditure> GetProjectExpenditures(string projectId, BudgetCategory category)
        {
            return Expenditures.Where(q => q.ProjectId == projectId && q.BudgetCategory == (int)category).OrderBy(q => q.CreateTime).ToArray();
        }

        public PageList<Comment> GetProjectCommentPageList(string projectId, int pageIndex, int pageSize)
        {
            return Comments.Where(c => c.ProjectId == projectId)
                .OrderByDescending(q => q.CreateTime)
                .PageList(pageIndex, pageSize);
        }

        public PageList<ProjectChangeLog> GetProjectChangeLogPageList(string projectId, int pageIndex, int pageSize, bool containsFinance = false)
        {
            var query = ProjectChangeLogs.Where(q => q.ProjectId == projectId);
            if (!containsFinance)
            {
                query = query.Where(q => q.ActionType < 100);
            }
            return query.OrderByDescending(q => q.CreateTime)
              .PageList(pageIndex, pageSize);
        }

        public Dictionary<int, int> GetWaitCheckStatusGroupCount()
        {
            return Projects.Where(p => p.Status == (int)ProjectStatus.PublishedWaitCheck
                || p.Status == (int)ProjectStatus.CompletedWaitCheck)
                .GroupBy(p => p.Status)
                .Select(g => new
                {
                    Key = g.Key,
                    Count = g.Count()
                }).ToDictionary(r => r.Key, r => r.Count);
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
            using (TransactionScope ts = new TransactionScope())
            {
                DbContext.Database.ExecuteSqlCommand("Delete Attachments Where ProjectId = @p0", new SqlParameter { ParameterName = "p0", Value = project.Id });
                DbContext.Database.ExecuteSqlCommand("Delete Comments Where ProjectId = @p0", new SqlParameter { ParameterName = "p0", Value = project.Id });
                DbContext.Database.ExecuteSqlCommand("Delete ProjectViewStatuses Where ProjectId = @p0", new SqlParameter { ParameterName = "p0", Value = project.Id });
                DbContext.Database.ExecuteSqlCommand("Delete Budgets Where ProjectId = @p0", new SqlParameter { ParameterName = "p0", Value = project.Id });
                DbContext.Database.ExecuteSqlCommand("Delete Expenditures Where ProjectId = @p0", new SqlParameter { ParameterName = "p0", Value = project.Id });

                DeleteEntity(project);
                SaveChanges();

                ts.Complete();
            }
        }


        public void TopProject(string[] projectIds)
        {
            if (projectIds != null && projectIds.Length > 0)
            {
                using (var ts = new TransactionScope())
                {
                    var maxSortIndex = Projects.Max(p => p.SortIndex);
                    var parameterName = "@p";
                    SqlParameter[] parameters = new SqlParameter[projectIds.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = new SqlParameter { ParameterName = parameterName + i, Value = projectIds[i] };
                    }

                    var sql = string.Format("Update Projects Set SortIndex = {0} Where Id In ({1})",
                        maxSortIndex + 1,
                        string.Join(",", parameters.Select(p => p.ParameterName)));

                    DbContext.Database.ExecuteSqlCommand(sql, parameters);

                    ts.Complete();
                }
            }
        }

        public Budget AddBudget(Budget budget)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                if (Budgets.Where(q => q.ProjectId == budget.ProjectId && q.Category == budget.Category).Count() > 0)
                {
                    throw new BusinessException(string.Format("已经存在{0}类型的预算记录", budget.CategoryString));
                }
                budget.Id = Guid.NewGuid().ToString();
                AddEntity(budget);
                SaveChanges();

                ts.Complete();

                return budget;
            }
        }

        public Budget EditBudget(Budget budget)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var totalCount = GetExpendituresTotalCount(budget.ProjectId, budget.Category);
                if (budget.Amount < totalCount)
                {
                    throw new BusinessException(string.Format("{0}预算的支出记录总金额超过预算设定的可用金额", budget.CategoryString));
                }
                UpdateEntity(budget);
                SaveChanges();

                ts.Complete();

                return budget;
            }
        }

        public Expenditure GetExpenditure(string expenditureId)
        {
            return Expenditures.FirstOrDefault(q => q.Id == expenditureId);
        }


        public Expenditure AddExpenditure(Expenditure expenditure)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var budget = Budgets.First(q => q.ProjectId == expenditure.ProjectId && q.Category == expenditure.BudgetCategory);
                var totalCount = GetExpendituresTotalCount(expenditure.ProjectId, expenditure.BudgetCategory);
                if (totalCount + expenditure.Count > budget.Amount)
                {
                    throw new BusinessException("增加的支出金额大于该类预算的可用金额");
                }

                expenditure.Id = Guid.NewGuid().ToString();
                expenditure.CreateTime = DateTime.Now;
                AddEntity(expenditure);
                SaveChanges();

                ts.Complete();
            }
            return expenditure;
        }

        public void DeleteExpenditure(Expenditure expenditure)
        {
            DeleteEntity(expenditure);
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

        public ProjectChangeLog AddChangeLog(ProjectChangeLog log)
        {
            log.Id = Guid.NewGuid().ToString();
            log.CreateTime = DateTime.Now;

            ProjectChangeLogs.Add(log);
            SaveChanges();

            UpdateLatestNews(log);

            return log;
        }

        public IEnumerable<ProjectViewStatusDetail> GetChangedProjectViewStatusDetail(string userId, bool includeFinanceStatus)
        {
            var viewStatusQuery = ProjectViewStatuses.Where(q => q.UserId == userId);
            if (includeFinanceStatus)
            {
                viewStatusQuery = viewStatusQuery.Where(q => q.Status > 0 || q.FinanceStatus > 0);
            }
            else
            {
                viewStatusQuery = viewStatusQuery.Where(q => q.Status > 0);
            }
            var query = viewStatusQuery.Join(Projects, vs => vs.ProjectId, p => p.Id, (vs, p) => new ProjectViewStatusDetail
            {
                Id = vs.Id,
                ProjectId = vs.ProjectId,
                ProjectName = p.Name,
                ProjectStatus = p.Status,
                UserId = vs.UserId,
                Status = vs.Status,
                FinanceStatus = vs.FinanceStatus,
                UpdateTime = vs.UpdateTime
            }).OrderByDescending(q => q.UpdateTime);

            return query.ToArray();
        }

        public void InitProjectCode()
        {
            var projects = Projects.OrderBy(q => q.CreateTime).ToArray();
            foreach (var project in projects)
            {
                SetProjectCode(project);
                UpdateEntity(project);
                SaveChanges();
            }
        }


        #endregion

    }
}
