﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class UserInfo
    {
        public string UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public RightDetail RightDetail
        {
            get;
            set;
        }

    }
}