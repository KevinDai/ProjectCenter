using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class UserListItem
    {
        public string Id
        {
            get;
            set;
        }

        public string LoginName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int RightLevel
        {
            get;
            set;
        }


        public UserListItem(User user)
        {
            Id = user.Id;
            LoginName = user.LoginName;
            Name = user.Name;
            RightLevel = user.RightLevel;
        }


    }
}