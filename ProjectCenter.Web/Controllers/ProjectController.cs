﻿using ProjectCenter.Models;
using ProjectCenter.Services;
using ProjectCenter.Services.Specifications.Projects;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;
using ProjectCenter.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    public class ProjectController : BaseController
    {

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
                                if (UserInfo.RightLevel != RightLevel.Administrator)
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
                                        temp = new ParticipantIdsSpecification(UserInfo.UserId);
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

        public SortDescriptor<Project>[] BuildProjectSortDescriptor(QueryFilter queryFilter)
        {
            List<SortDescriptor<Project>> sort = new List<SortDescriptor<Project>>();
            if (queryFilter != null
                && queryFilter.SortFields != null
                && queryFilter.SortFields.Any())
            {
                foreach (var sortField in queryFilter.SortFields)
                {
                    sort.Add(new SortDescriptor<Project>(sortField.Field, sortField.IsAsc ? ListSortDirection.Ascending : ListSortDirection.Descending));
                    //switch (sortField.Field)
                    //{
                    //    case "Deadline":
                    //        sort.Add(new SortDescriptor<Project>(p => p.Deadline));
                    //        break;
                    //}
                }
            }

            return sort.Count == 0
                ?
                new SortDescriptor<Project>[] { new SortDescriptor<Project>("Deadline") }
                :
                sort.ToArray();
        }

        #endregion

        #region 操作

        //
        // GET: /Project/
        [HttpGet]
        public ActionResult List()
        {
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

            var model = ProjectService.GetProjectPageList(spec, sort, queryFilter.PageIndex, queryFilter.PageSize);

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

            var model = new ProjectViewModel(project);
            model.Attachments = ProjectService.GetProjectAttachments(projectId);
            model.CommentPageList = ProjectService.GetProjectCommentPageList(projectId, 1, 20);

            return JsonMessageResult(model);
        }


        [HttpPost]
        public ActionResult AddProject(Project project, string[] attachmentIds)
        {
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult EditProject(Project project)
        {
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult AddAttachment(string projectId)
        {
            return JsonMessageResult(null);
        }

        [HttpPost]
        public ActionResult AddComment(string projectId)
        {
            return JsonMessageResult(null);
        }

        #endregion

    }
}
