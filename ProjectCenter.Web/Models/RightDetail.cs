using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class RightDetail
    {

        public RightDetail(int rightLevel)
        {
            EnableViewList = rightLevel < 4;
            EnableViewDetail = rightLevel < 3;
            EnableEditProject = rightLevel < 2;
            EnableCheckProject = rightLevel < 2;
            EnableSetProjectUser = rightLevel < 2;
            EnbaleDeleteProject = rightLevel < 2;
        }

        public bool EnableViewList
        {
            get;
            set;
        }

        public bool EnableViewDetail
        {
            get;
            set;
        }

        public bool EnableCheckProject
        {
            get;
            set;
        }

        public bool EnableEditProject
        {
            get;
            set;
        }

        public bool EnableSetProjectUser
        {
            get;
            set;
        }

        public bool EnbaleDeleteProject
        {
            get;
            set;
        }

    }
}