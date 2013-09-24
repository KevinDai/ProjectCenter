﻿using Newtonsoft.Json;
using ProjectCenter.Models;
using ProjectCenter.Services;
using ProjectCenter.Services.Specifications.Projects;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;
using ProjectCenter.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    public class ProjectController : BaseController
    {
        private const string AttachmentFolder = "Attachments";

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
                    }
                    if (temp != null)
                    {
                        spec = spec == null ? temp : new AndSpecification<Project>(spec, temp);
                    }
                }
            }
            return spec;
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

            var folder = Server.MapPath(string.Format("{0}\\{1}", AttachmentFolder, projectId));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = folder + "\\" + fileName;
            var i = 1;
            while (System.IO.File.Exists(filePath))
            {
                var index = fileName.LastIndexOf('.');
                var temp = fileName.Insert(index - 1, (i).ToString());
                filePath = Server.MapPath(string.Format("{0}\\{1}\\{2}", AttachmentFolder, projectId, temp));
                i++;
            }
            return filePath;
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

        public ActionResult LoadProjects(QueryFilter queryFilter)
        {
            if (queryFilter == null)
            {
                throw new Exception("查询条件对象不能为空");
            }

            ISpecification<Project> spec = BuildProjectSpecification(queryFilter);
            SortDescriptor<Project>[] sort = BuildProjectSortDescriptor(queryFilter);

            var result = ProjectService.GetProjectPageList(spec, sort, queryFilter.PageIndex, queryFilter.PageSize);

            var model = new PageList<ProjectListItemViewModel>(
                result.List.Select(item => new ProjectListItemViewModel(item, UserInfo)),
                result.PageIndex, result.PageSize, result.Total);

            return JsonMessageResult(model);
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

            return JsonMessageResult(model);
        }

        [HttpPost]
        public ActionResult CheckProjects(string[] projectIds, int status)
        {
            if (!UserInfo.RightDetail.EnableCheckProject)
            {
                throw new BusinessException("无审核操作权限");
            }

            ProjectService.CheckProjects(projectIds, (ProjectStatus)status);

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

            ProjectService.CheckProjects(new string[] { projectId }, ProjectStatus.CompletedWaitCheck);

            return JsonMessageResult((int)ProjectStatus.CompletedWaitCheck);
        }

        [HttpPost]
        public ActionResult AddProject(Project project, string[] attachmentIds)
        {
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult EditProject(Project project)
        {
            if (string.IsNullOrEmpty(project.Id))
            {
                project.CreatorId = UserInfo.UserId;
                project.CreatorName = UserInfo.UserName;
                //var entity = ProjectService.GetProject(project.Id);
                //UpdateModel(entity, "project");
                project = ProjectService.AddProject(project);
            }
            else
            {
                var entity = ProjectService.GetProject(project.Id);
                UpdateModel(entity, "project");
                project = ProjectService.UpdateProject(entity);
            }

            return JsonMessageResult(new ProjectEditViewModel(project, UserInfo));
        }

        public ActionResult DeleteProject(string projectId)
        {
            var project = ProjectService.GetProject(projectId);
            if (project == null)
            {
                throw new BusinessException("操作的项目不存在");
            }
            if (!UserInfo.RightDetail.EnbaleDeleteProject)
            {
                throw new BusinessException("无删除操作权限");
            }

            ProjectService.DeleteProject(project);

            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult AddAttachment()
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

            return JsonMessageResult(null);
        }

        public ActionResult LoadAttachments(string projectId)
        {
            var comments = ProjectService.GetProjectAttachments(projectId);
            return JsonMessageResult(comments);
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

            if (!UserInfo.RightDetail.EnbaleDeleteProject)
            {
                throw new BusinessException("无删除操作权限");
            }
            ProjectService.DeleteComment(comment);

            return JsonMessageResult(null);
        }

        #endregion

    }
}
