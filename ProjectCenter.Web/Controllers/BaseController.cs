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
        protected UserInfo UserInfo
        {
            get;
            set;
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            UserInfo = new UserInfo()
            {
                UserId = "test",
                UserName = "测试用户",
                IsProjectAdmin = false,
                IsSystemAdmin = true
            };

        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewBag.UserInfo = UserInfo;
            //if (filterContext.Result is ViewResult)
            //{
            //    var result = filterContext.Result as ViewResult;
            //    result.mo
            //}
        }

        protected ActionResult JsonMessageResult(object data)
        {
            return Json(new JsonMessage(data));
        }

    }
}
