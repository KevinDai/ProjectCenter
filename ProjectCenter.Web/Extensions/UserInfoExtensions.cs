using ProjectCenter.Models;
using ProjectCenter.Services.Specifications.Projects;
using ProjectCenter.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Extensions
{
    public static class UserInfoExtensions
    {

        private static bool ParamatersValid(UserInfo user, Project project)
        {
            if (project == null || user == null)
            {
                return false;
            }
            return true;
        }

        public static bool EnableViewDetail(this UserInfo user, Project project)
        {
            return ParamatersValid(user, project) && (
                user.RightDetail.EnableViewDetail
                || project.CreatorId == user.UserId && project.Status == (int)ProjectStatus.PublishedWaitCheck
                || new ManagerIdsSpecification(user.UserId).SatisfiedBy().Compile()(project)
                || new ParticipantIdsSpecification(user.UserId).SatisfiedBy().Compile()(project));
        }

        public static bool EnableSetCompleteCheck(this UserInfo user, Project project)
        {
            return ParamatersValid(user, project) && (
                new ManagerIdsSpecification(user.UserId).SatisfiedBy().Compile()(project)
                && project.Status == (int)ProjectStatus.PublishedAndChecked);

        }

        public static bool EnableEditProject(this UserInfo user, Project project)
        {
            return ParamatersValid(user, project) && (
                user.RightDetail.EnableEditProject
                || project.CreatorId == user.UserId && project.Status == (int)ProjectStatus.PublishedWaitCheck
                || new ManagerIdsSpecification(user.UserId).SatisfiedBy().Compile()(project)
                || new ParticipantIdsSpecification(user.UserId).SatisfiedBy().Compile()(project));
        }

    }
}