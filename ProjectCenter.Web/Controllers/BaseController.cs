using ProjectCenter.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            ViewBag.User = new UserInfo()
            {
                UserId = "test",
                UserName = "测试用户",
                RightLevel = RightLevel.Common
            };
        }
    }
}
