using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectCenter.Models;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;
using ProjectCenter.Services.Models;

namespace ProjectCenter.Services
{
    public interface IProjectService
    {
        PageList<Project> GetProjectPageList(ISpecification<Project> specs, SortDescriptor<Project>[] sorts, int pageIndex, int pageSize);

        IEnumerable<ProjectStatisticItem> GetProjectStatistics(ISpecification<Project> spec);

        Project GetProject(string projectId);

        void UpdateProjectViewStatusRead(string projectId, string userId);

        void UpdateAllProjectViewStatusRead(string userId);

        IEnumerable<ProjectViewStatus> GetProjectViewStatus(string[] projectIds, string userId);

        IEnumerable<Attachment> GetProjectAttachments(string projectId);

        IEnumerable<Budget> GetProjectBudgets(string projectId);

        IEnumerable<Expenditure> GetProjectExpenditures(string projectId);

        IEnumerable<Expenditure> GetProjectExpenditures(string projectId, BudgetCategory category);

        Dictionary<int, int> GetWaitCheckStatusGroupCount();

        PageList<Comment> GetProjectCommentPageList(string projectId, int pageIndex, int pageSize);

        PageList<ProjectChangeLog> GetProjectChangeLogPageList(string projectId, int pageIndex, int pageSize, bool containsFinance = false);

        Project AddProject(Project project);

        void CheckProjects(string[] projectIds, ProjectStatus status);

        Project UpdateProject(Project project);

        void DeleteProject(Project project);

        void TopProject(string[] projectIds);

        Budget AddBudget(Budget budget);

        Budget EditBudget(Budget budget);

        Expenditure GetExpenditure(string expenditureId);

        Expenditure AddExpenditure(Expenditure expenditure);

        void DeleteExpenditure(Expenditure expenditure);

        Attachment AddAttachment(Attachment attachment);

        Attachment GetAttachment(string attachmentId);

        IEnumerable<Attachment> GetAttachments(string[] attachmentIds);

        void DeleteAttachment(Attachment attachment);

        Comment AddComment(Comment comment);

        Comment GetComment(string commentId);

        void DeleteComment(Comment comment);

        ProjectChangeLog AddChangeLog(ProjectChangeLog log);

        IEnumerable<ProjectViewStatusDetail> GetChangedProjectViewStatusDetail(string userId, bool includeFinanceStatus);
    }
}
