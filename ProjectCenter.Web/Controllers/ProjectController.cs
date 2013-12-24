using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using ProjectCenter.Models;
using ProjectCenter.Services;
using ProjectCenter.Services.Specifications.Projects;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;
using ProjectCenter.Web.Exports;
using ProjectCenter.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCenter.Web.Extensions;
using System.Transactions;

namespace ProjectCenter.Web.Controllers
{
    public class ProjectController : SecurityController
    {
        public const string AttachmentFolder = "\\Attachments";
        public const string TempFilesFolder = "\\TempFiles";
        public const int ExportPageSize = 2000;

        private IProjectService _projectService;
        public IProjectService ProjectService
        {
            get
            {
                if (_projectService == null)
                {
                    _projectService = ServiceFactory.Instance.CreateProjectService();
                }
                return _projectService;
            }
            set
            {
                _projectService = value;
            }
        }

        #region 方法

        private ISpecification<Project> BuildProjectSpecification(QueryFilter queryFilter)
        {
            ISpecification<Project> spec = null;
            if (queryFilter != null
                && queryFilter.FieldFilters != null
                && queryFilter.FieldFilters.Any())
            {
                foreach (var fieldFilter in queryFilter.FieldFilters)
                {
                    ISpecification<Project> temp = null;
                    switch (fieldFilter.Field)
                    {
                        //我的项目
                        case "RelationType":
                            if (string.IsNullOrWhiteSpace(fieldFilter.Value))
                            {
                                //如果非管理员级别，增加默认的过滤条件，只能查看与自己相关的项目
                                if (!UserInfo.RightDetail.EnableViewList)
                                {
                                    temp = new OrSpecification<Project>(new CreatorIdSpecification(UserInfo.UserId), new ManagerIdsSpecification(UserInfo.UserId));
                                    temp = new OrSpecification<Project>(temp, new ParticipantIdsSpecification(UserInfo.UserId));
                                }
                            }
                            else
                            {
                                switch (fieldFilter.Value)
                                {
                                    case "0":
                                        temp = new CreatorIdSpecification(UserInfo.UserId);
                                        break;
                                    case "1":
                                        temp = new ManagerIdsSpecification(UserInfo.UserId);
                                        break;
                                    case "2":
                                        temp = new OrSpecification<Project>(new CreatorIdSpecification(UserInfo.UserId), new ManagerIdsSpecification(UserInfo.UserId));
                                        temp = new OrSpecification<Project>(temp, new ParticipantIdsSpecification(UserInfo.UserId));
                                        //temp = new OrSpecification<Project>(new ManagerIdsSpecification(UserInfo.UserId), new ParticipantIdsSpecification(UserInfo.UserId));
                                        break;
                                }
                            }
                            break;
                        case "Status":
                            if (!string.IsNullOrWhiteSpace(fieldFilter.Value))
                            {
                                temp = new StatusSpecification(Int32.Parse(fieldFilter.Value));
                            }
                            break;
                        case "Type":
                            if (!string.IsNullOrWhiteSpace(fieldFilter.Value))
                            {
                                temp = new TypeSpecification(Int32.Parse(fieldFilter.Value));
                            }
                            break;
                        case "ProjectName":
                            if (!string.IsNullOrWhiteSpace(fieldFilter.Value))
                            {
                                temp = new NameSpecification(fieldFilter.Value);
                            }
                            break;
                        case "ManagerId":
                            if (!string.IsNullOrWhiteSpace(fieldFilter.Value))
                            {
                                temp = new ManagerIdsSpecification(fieldFilter.Value);
                            }
                            break;
                        case "ParticipantId":
                            if (!string.IsNullOrWhiteSpace(fieldFilter.Value))
                            {
                                temp = new ParticipantIdsSpecification(fieldFilter.Value);
                            }
                            break;
                        case "StartTime":
                            {
                                DateTime? min = null;
                                DateTime? max = null;
                                if (TryGetDateTimeSpanValue(fieldFilter.Value, ref min, ref max))
                                {
                                    temp = new StartTimeSpecification(min, max);
                                }
                            }
                            break;
                        case "Deadline":
                            {
                                DateTime? min = null;
                                DateTime? max = null;
                                if (TryGetDateTimeSpanValue(fieldFilter.Value, ref min, ref max))
                                {
                                    temp = new DeadlineSpecification(min, max);
                                }
                            }
                            break;
                    }
                    if (temp != null)
                    {
                        spec = spec == null ? temp : new AndSpecification<Project>(spec, temp);
                    }
                }
            }
            return spec;
        }

        private bool TryGetDateTimeSpanValue(string value, ref DateTime? min, ref DateTime? max)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                DateTime tempDateTime;
                var spans = value.Split(',');

                if (spans.Length > 0 && DateTime.TryParse(spans[0], out tempDateTime))
                {
                    min = tempDateTime;
                }
                if (spans.Length > 1 && DateTime.TryParse(spans[1], out tempDateTime))
                {
                    max = tempDateTime;
                }

                return min.HasValue || max.HasValue;
            }
            return false;
        }

        private SortDescriptor<Project>[] BuildProjectSortDescriptor(QueryFilter queryFilter)
        {
            List<SortDescriptor<Project>> sort = new List<SortDescriptor<Project>>();
            if (queryFilter != null
                && queryFilter.SortFields != null
                && queryFilter.SortFields.Any())
            {
                foreach (var sortField in queryFilter.SortFields)
                {
                    //sort.Add(new SortDescriptor<Project>(sortField.Field, sortField.IsAsc ? ListSortDirection.Ascending : ListSortDirection.Descending));
                    switch (sortField.Field)
                    {
                        case "Deadline":
                            sort.Add(SortDescriptor<Project>.CreateSortDescriptor(p => p.Deadline));
                            break;
                    }
                }
            }

            return sort.Count == 0
                ?
                new SortDescriptor<Project>[] { SortDescriptor<Project>.CreateSortDescriptor(p => p.Deadline) }
                //new SortDescriptor<Project>[] { new SortDescriptor<Project>("Deadline") }
                :
                sort.ToArray();
        }

        private string GetAttachmentPath(string projectId, string fileName)
        {

            var folder = GetProjectAttachmentsFolder(projectId);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = folder + "\\" + fileName;
            var i = 1;
            while (System.IO.File.Exists(filePath))
            {
                var index = fileName.LastIndexOf('.');
                var temp = fileName.Insert(index, (i).ToString());
                filePath = Server.MapPath(string.Format("{0}\\{1}\\{2}", AttachmentFolder, projectId, temp));
                i++;
            }
            return filePath;
        }

        private string GetAttachmentZipFilePath(string zipFile)
        {
            return Server.MapPath(string.Format("{0}\\{1}", TempFilesFolder, zipFile));
        }

        private string GetProjectAttachmentsFolder(string projectId)
        {
            return Server.MapPath(string.Format("{0}\\{1}", AttachmentFolder, projectId));
        }

        private string EncodeDownloadFileName(string name)
        {
            return HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8);
        }

        private void AddChangeLog(string projectId, ProjectActionType actionType, string message)
        {
            ProjectService.AddChangeLog(new ProjectChangeLog()
            {
                ActionType = (int)actionType,
                UserId = UserInfo.UserId,
                UserName = UserInfo.UserName,
                ProjectId = projectId,
                Message = message
            });
        }

        private BudgetViewModel[] GetBudgetViewModels(string projectId)
        {
            var budgets = ProjectService.GetProjectBudgets(projectId)
                .Select(b => new BudgetViewModel(b))
                .ToDictionary(b => b.Category, b => b);
            var categorys = Enum.GetValues(typeof(BudgetCategory));
            foreach (int category in categorys)
            {
                if (!budgets.ContainsKey(category))
                {
                    var entity = new Budget()
                    {
                        ProjectId = projectId,
                        Category = category,
                        Amount = 0
                    };
                    entity = ProjectService.AddBudget(entity);
                    budgets.Add(entity.Category, new BudgetViewModel(entity));
                }
            }
            var expenditures = ProjectService.GetProjectExpenditures(projectId);
            BudgetViewModel budget = null;
            foreach (var expenditure in expenditures)
            {
                if (budgets.TryGetValue(expenditure.BudgetCategory, out budget))
                {
                    budget.Expenditures.Add(expenditure);
                }
            }
            return budgets.Values.OrderBy(q => q.Category).ToArray();
        }

        private void FinanceManageRightValid()
        {
            if (!UserInfo.RightDetail.EnableManageFinance)
            {
                throw new BusinessException("无操作财务信息权限");
            }
        }

        #endregion

        #region 操作

        //
        // GET: /Project/
        [HttpGet]
        public ActionResult List()
        {
            ViewBag.Users = GetUsers().Select(u => new ItemSelectViewModel(u.Id, u.Name)).ToArray();
            return View();
        }

        [HttpPost]
        public ActionResult LoadProjects(QueryFilter queryFilter)
        {
            if (queryFilter == null)
            {
                throw new Exception("查询条件对象不能为空");
            }

            ISpecification<Project> spec = BuildProjectSpecification(queryFilter);
            SortDescriptor<Project>[] sort = BuildProjectSortDescriptor(queryFilter);

            var result = ProjectService.GetProjectPageList(spec, sort, queryFilter.PageIndex, queryFilter.PageSize);
            var viewStatus = ProjectService.GetProjectViewStatus(result.List.Select(q => q.Id).ToArray(), UserInfo.UserId)
                .ToDictionary(q => q.ProjectId, q => q);

            ProjectViewStatus temp = null;
            var model = new PageList<ProjectListItemViewModel>(
                result.List.Select(item =>
                {
                    viewStatus.TryGetValue(item.Id, out temp);
                    return new ProjectListItemViewModel(item, UserInfo, temp);
                }),
                result.PageIndex, result.PageSize, result.Total);

            return JsonMessageResult(model);
        }

        [HttpPost]
        public ActionResult ExportProjects(QueryFilter queryFilter)
        {
            var export = new ProjectExprot();

            ISpecification<Project> spec = BuildProjectSpecification(queryFilter);
            SortDescriptor<Project>[] sort = new SortDescriptor<Project>[] { 
                SortDescriptor<Project>.CreateSortDescriptor(p => p.Type) 
            };

            PageList<Project> pageList = null;
            int pageIndex = 1;
            int pageSize = ExportPageSize;
            do
            {
                pageList = ProjectService.GetProjectPageList(spec, sort, pageIndex, pageSize);
                foreach (var project in pageList.List)
                {
                    if (UserInfo.EnableViewDetail(project))
                    {
                        export.WriteProject(project);
                    }
                }
            } while (pageList.Total > pageIndex++ * pageSize);
            export.GroupProjectTypeColumn();

            var fileName = Guid.NewGuid().ToString() + ".xls";
            var xlsFile = Server.MapPath(string.Format("{0}\\{1}", TempFilesFolder, fileName));
            export.SaveToFile(xlsFile);

            return JsonMessageResult(fileName);
        }

        public ActionResult DownloadProjectExportFile(string path)
        {
            var fullPath = Server.MapPath(string.Format("{0}\\{1}", TempFilesFolder, path));
            return File(fullPath, "application/vnd.ms-excel", EncodeDownloadFileName("工作任务清单.xls"));
        }

        [HttpPost]
        public ActionResult GetProject(string projectId)
        {
            var project = ProjectService.GetProject(projectId);
            if (project == null)
            {
                throw new BusinessException("指定的项目不存在");
            }

            var model = new ProjectEditViewModel(project, UserInfo);
            model.Attachments = ProjectService.GetProjectAttachments(projectId);
            model.CommentPageList = ProjectService.GetProjectCommentPageList(projectId, 1, 20);
            if (model.EnableManageFinance)
            {
                model.Budgets = GetBudgetViewModels(project.Id);
            }

            ProjectService.UpdateProjectViewStatusRead(projectId, UserInfo.UserId);

            return JsonMessageResult(model);
        }

        [HttpPost]
        public ActionResult CheckProjects(string[] projectIds, int status)
        {
            if (!UserInfo.RightDetail.EnableCheckProject)
            {
                throw new BusinessException("无审核操作权限");
            }

            var projectStatus = (ProjectStatus)status;
            ProjectService.CheckProjects(projectIds, projectStatus);

            foreach (var projectId in projectIds)
            {
                AddChangeLog(projectId, ProjectActionType.ChangeStatus, Project.GetStatusString(projectStatus));
            }

            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult SendFinishCheck(string projectId)
        {
            var project = ProjectService.GetProject(projectId);
            if (project == null)
            {
                throw new BusinessException("指定的项目不存在");
            }
            if (!new ManagerIdsSpecification(UserInfo.UserId).SatisfiedBy().Compile()(project))
            {
                throw new BusinessException("无提交完成待审的操作权限");
            }
            if (project.Status != (int)ProjectStatus.PublishedAndChecked)
            {
                throw new BusinessException("指定的项目的当前状态不能提交完成待审");
            }

            var status = ProjectStatus.CompletedWaitCheck;
            ProjectService.CheckProjects(new string[] { projectId }, status);

            AddChangeLog(projectId, ProjectActionType.ChangeStatus, Project.GetStatusString(status));

            return JsonMessageResult((int)ProjectStatus.CompletedWaitCheck);
        }

        [HttpPost]
        public ActionResult AddProject(Project project, string[] attachmentIds)
        {
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult EditProject(Project project, string remark)
        {
            if (string.IsNullOrEmpty(project.Id))
            {
                project.CreatorId = UserInfo.UserId;
                project.CreatorName = UserInfo.UserName;
                //var entity = ProjectService.GetProject(project.Id);
                //UpdateModel(entity, "project");
                project = ProjectService.AddProject(project);

                AddChangeLog(project.Id, ProjectActionType.Create, remark);
            }
            else
            {
                var entity = ProjectService.GetProject(project.Id);
                UpdateModel(entity, "project");
                project = ProjectService.UpdateProject(entity);

                AddChangeLog(project.Id, ProjectActionType.Update, remark);
            }

            return JsonMessageResult(new ProjectEditViewModel(project, UserInfo));
        }

        [HttpPost]
        public ActionResult DeleteProject(string projectId)
        {
            var project = ProjectService.GetProject(projectId);
            if (project != null)
            {
                if (!UserInfo.EnableDelete(project))
                {
                    throw new BusinessException("无删除操作权限");
                }

                ProjectService.DeleteProject(project);

                var attachmentsFolder = GetProjectAttachmentsFolder(projectId);
                if (Directory.Exists(attachmentsFolder))
                {
                    Directory.Delete(attachmentsFolder, true);
                }

                AddChangeLog(project.Id, ProjectActionType.Delete, project.Name);
            }

            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult LoadBudgets(string projectId)
        {
            FinanceManageRightValid();

            var budgets = GetBudgetViewModels(projectId);
            return JsonMessageResult(budgets);
        }

        [HttpPost]
        public ActionResult EditBudgets(string projectId, BudgetEditItemViewModel[] items)
        {
            FinanceManageRightValid();

            if (items != null && items.Any())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var project = ProjectService.GetProject(projectId);
                    if (project == null)
                    {
                        throw new BusinessException("指定的项目不存在");
                    }
                    if (!project.NeedManageFinance())
                    {
                        throw new BusinessException("指定的项目不需要进行财务管理");
                    }

                    var count = items.Sum(q => q.Amount);
                    if (project.Amount < count)
                    {
                        throw new BusinessException("设定的分类预算总额大于指定的项目的金额");
                    }

                    var budgets = ProjectService.GetProjectBudgets(projectId)
                        .ToDictionary(b => b.Category, b => b);

                    Budget budget = null;
                    foreach (var item in items)
                    {
                        if (budgets.TryGetValue(item.Category, out budget))
                        {
                            if (budget.Amount != item.Amount)
                            {
                                budget.Amount = item.Amount;
                                ProjectService.EditBudget(budget);
                            }
                        }
                        else
                        {
                            budget = new Budget()
                            {
                                ProjectId = projectId,
                                Category = item.Category,
                                Amount = item.Amount
                            };
                            ProjectService.AddBudget(budget);
                        }
                    }
                    ts.Complete();
                }

                AddChangeLog(projectId, ProjectActionType.EditBudget, string.Empty);
            }
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult AddExpenditure(Expenditure expenditure)
        {
            FinanceManageRightValid();

            expenditure.CreatorId = UserInfo.UserId;
            expenditure.CreatorName = UserInfo.UserName;
            ProjectService.AddExpenditure(expenditure);

            AddChangeLog(expenditure.ProjectId, ProjectActionType.AddExpenditure, expenditure.BudgetCategoryString);

            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult DeleteExpenditure(string expenditureId)
        {
            FinanceManageRightValid();
            var expenditure = ProjectService.GetExpenditure(expenditureId);
            if (expenditure != null)
            {
                ProjectService.DeleteExpenditure(expenditure);
                AddChangeLog(expenditure.ProjectId, ProjectActionType.DeleteExpenditure, expenditure.BudgetCategoryString);
            }
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult LoadExpenditure(string projectId, int category)
        {
            FinanceManageRightValid();

            var expenditures = ProjectService.GetProjectExpenditures(projectId, (BudgetCategory)category);
            return JsonMessageResult(expenditures);
        }

        [HttpPost]
        public ActionResult UploadAttachment()
        {
            var projectId = Request.Params["projectId"];
            HttpPostedFileBase file = Request.Files["Filedata"];
            var path = GetAttachmentPath(projectId, file.FileName);
            file.SaveAs(path);

            var attachment = new Attachment();
            attachment.Name = file.FileName;
            attachment.Path = path;
            attachment.ProjectId = projectId;
            attachment.UploadUserId = UserInfo.UserId;
            attachment.UploadUserName = UserInfo.UserName;
            ProjectService.AddAttachment(attachment);

            AddChangeLog(projectId, ProjectActionType.AddAttachment, file.FileName);

            return JsonMessageResult(null);
        }

        public ActionResult LoadAttachments(string projectId)
        {
            var attachments = ProjectService.GetProjectAttachments(projectId);
            return JsonMessageResult(attachments);
        }

        public ActionResult DownloadAttachment(string id)
        {
            var attachment = ProjectService.GetAttachment(id);
            if (attachment == null || !System.IO.File.Exists(attachment.Path))
            {
                return Content("指定的附件不存在");
            }

            return File(attachment.Path, "text/plain", EncodeDownloadFileName(attachment.Name));
        }

        public ActionResult ZipAttachments(string[] ids)
        {
            string zipFile = string.Empty;
            if (ids != null && ids.Length > 0)
            {
                var attachments = ProjectService.GetAttachments(ids).Where(a => System.IO.File.Exists(a.Path)).ToArray();
                if (attachments.Any())
                {
                    zipFile = Guid.NewGuid().ToString() + ".zip";
                    var zipFilePath = GetAttachmentZipFilePath(zipFile);
                    using (var stream = System.IO.File.Create(zipFilePath))
                    {
                        using (var s = new ZipOutputStream(stream))
                        {
                            s.SetLevel(9);
                            foreach (var attachment in attachments)
                            {
                                byte[] bytes = System.IO.File.ReadAllBytes(attachment.Path);
                                var entry = new ZipEntry(new FileInfo(attachment.Path).Name);
                                s.PutNextEntry(entry);
                                s.Write(bytes, 0, bytes.Length);
                            }
                            s.Finish();
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(zipFile))
            {
                throw new BusinessException("指定的附件不存在，请刷新附件列表");
            }

            return JsonMessageResult(zipFile);
        }

        public ActionResult DownloadAttachmentZipFile(string path)
        {
            var fullPath = GetAttachmentZipFilePath(path);
            return File(fullPath, "application/zip", EncodeDownloadFileName("项目附件.zip"));
        }

        [HttpPost]
        public ActionResult LoadChangeMessages()
        {
            var viewStatus = ProjectService.GetChangedProjectViewStatusDetail(
                UserInfo.UserId, UserInfo.RightDetail.EnableManageFinance);
            var result = viewStatus.Select(vs => new ChangeMessageViewModel(UserInfo, vs)).ToArray();

            return JsonMessageResult(result);
        }

        [HttpPost]
        public ActionResult UpdateProjectViewStatusRead(string projectId)
        {
            ProjectService.UpdateProjectViewStatusRead(projectId, UserInfo.UserId);
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult UpdateAllProjectViewStatusRead()
        {
            ProjectService.UpdateAllProjectViewStatusRead(UserInfo.UserId);
            return JsonMessageResult(null);
        }


        //public ActionResult DownloadAttachments(string id)
        //{
        //    var project = ProjectService.GetProject(id);
        //    if (project == null)
        //    {
        //        return Content("指定的项目不存在");
        //    }

        //    var attachments = ProjectService.GetProjectAttachments(id);
        //    if (attachments == null || !attachments.Any())
        //    {
        //        return Content("指定的项目不存在附件");
        //    }

        //    var zipFileName = Server.MapPath(string.Format("{0}\\{1}.zip", TempFilesFolder, Guid.NewGuid().ToString()));
        //    using (var stream = System.IO.File.Create(zipFileName))
        //    {
        //        using (var s = new ZipOutputStream(stream))
        //        {
        //            s.SetLevel(9);
        //            foreach (var attachment in attachments)
        //            {
        //                byte[] bytes = System.IO.File.ReadAllBytes(attachment.Path);
        //                var entry = new ZipEntry(new FileInfo(attachment.Path).Name);
        //                s.PutNextEntry(entry);
        //                s.Write(bytes, 0, bytes.Length);
        //            }
        //            s.Finish();
        //        }
        //    }
        //    return File(zipFileName, "application/zip", string.Format("{0}附件.zip", project.Name));
        //}

        public ActionResult DeleteAttachment(string attachmentId)
        {
            var attachment = ProjectService.GetAttachment(attachmentId);
            if (attachment != null)
            {
                if (!UserInfo.RightDetail.EnbaleDeleteProject && UserInfo.UserId != attachment.UploadUserId)
                {
                    throw new BusinessException("无删除操作权限");
                }

                ProjectService.DeleteAttachment(attachment);

                if (System.IO.File.Exists(attachment.Path))
                {
                    System.IO.File.Delete(attachment.Path);
                }

                AddChangeLog(attachment.ProjectId, ProjectActionType.DeleteAttachment, attachment.Name);
            }

            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult AddComment(string projectId, string content)
        {
            var comment = new Comment();
            comment.ProjectId = projectId;
            comment.Content = content;
            comment.CreatorId = UserInfo.UserId;
            comment.CreatorName = UserInfo.UserName;

            ProjectService.AddComment(comment);

            AddChangeLog(projectId, ProjectActionType.AddComment, string.Empty);

            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult LoadComments(string projectId, int pageIndex, int pageSize)
        {
            var comments = ProjectService.GetProjectCommentPageList(projectId, pageIndex, pageSize);

            return JsonMessageResult(comments);
        }

        [HttpPost]
        public ActionResult DeleteComment(string commentId)
        {
            var comment = ProjectService.GetComment(commentId);
            if (comment == null)
            {
                throw new BusinessException("操作的评论不存");
            }

            if (!UserInfo.RightDetail.EnbaleDeleteProject && UserInfo.UserId != comment.CreatorId)
            {
                throw new BusinessException("无删除操作权限");
            }
            ProjectService.DeleteComment(comment);

            AddChangeLog(comment.ProjectId, ProjectActionType.DeleteComment, string.Empty);

            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult LoadProjectChangeLogs(string projectId, int pageIndex, int pageSize)
        {
            var changeLogs = ProjectService.GetProjectChangeLogPageList(projectId, pageIndex, pageSize, UserInfo.RightDetail.EnableManageFinance);

            return JsonMessageResult(changeLogs);
        }

        [HttpPost]
        public ActionResult LoadWaitCheckStatusGroupCount()
        {
            var result = ProjectService.GetWaitCheckStatusGroupCount();
            return JsonMessageResult(result);
        }

        #endregion

    }
}