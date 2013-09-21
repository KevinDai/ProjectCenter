﻿using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectCenter.Web.Extensions;
using ProjectCenter.Util;
using ProjectCenter.Services.Specifications.Projects;

namespace ProjectCenter.Web.Models
{
    public class ProjectListItemViewModel
    {
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime Deadline
        {
            get;
            set;
        }

        /// <summary>
        /// 负责人名称
        /// </summary>
        public string ManagerNames
        {
            get;
            set;
        }

        /// <summary>
        /// 参与者名称
        /// </summary>
        public string ParticipantNames
        {
            get;
            set;
        }

        /// <summary>
        /// 项目状态
        /// </summary>
        public int Status
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许查看详细信息
        /// </summary>
        public bool EnableViewDetail
        {
            get;
            set;
        }

        public ProjectListItemViewModel(Project project, UserInfo userInfo)
        {
            Preconditions.CheckNotNull(project, "project");
            Preconditions.CheckNotNull(userInfo, "userInfo");

            project.Map(this);

            EnableViewDetail = userInfo.RightDetail.EnableViewDetail
                || project.CreatorId == userInfo.UserId
                || new ManagerIdsSpecification(userInfo.UserId).SatisfiedBy().Compile()(project)
                || new ParticipantIdsSpecification(userInfo.UserId).SatisfiedBy().Compile()(project);
        }
    }
}