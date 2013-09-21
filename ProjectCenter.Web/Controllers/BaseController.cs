using ProjectCenter.Services;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Web.ActionResults;
using ProjectCenter.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ProjectCenter.Web.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        private IUserService _userService;
        public IUserService UserService
        {
            get
            {
                if (_userService == null)
                {
                    _userService = ServiceFactory.Instance.CreateUserService();
                }
                return _userService;
            }
            set
            {
                _userService = value;
            }
        }

        protected UserInfo UserInfo
        {
            get;
            set;
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                var user = UserService.GetUserById(User.Identity.Name);
                UserInfo = new UserInfo()
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    RightDetail = new RightDetail(user.RightLevel)
                };

            }

        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewBag.UserInfo = UserInfo;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is BusinessException)
            {
                filterContext.Result = new CommonJsonResult()
                {
                    Data = new JsonMessage()
                    {
                        Status = (int)MessageStatus.BusinessException,
                        Message = filterContext.Exception.Message
                    }
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        //protected override void Execute(System.Web.Routing.RequestContext requestContext)
        //{
        //    try
        //    {
        //        base.Execute(requestContext);
        //    }
        //    catch (BusinessException ex)
        //    {
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        protected ActionResult JsonMessageResult(object data)
        {
            return new CommonJsonResult()
                {
                    Data = new JsonMessage(data)
                };
        }

    }
}
