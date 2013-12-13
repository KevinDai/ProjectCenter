using ProjectCenter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    public class UserController : SecurityController
    {
        public ActionResult ChangePassword(string password)
        {
            UserService.ChangePassword(UserInfo.UserId, password);
            return JsonMessageResult(null);
        }
    }
}