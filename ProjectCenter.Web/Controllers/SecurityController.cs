using ProjectCenter.Models;
using ProjectCenter.Services;
using ProjectCenter.Util;
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
    public abstract class SecurityController : BaseController
    {
        private const string UsersCacheKey = "Users";
        private const string UserCachePrefix = "User_";

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
                var user = GetUser(User.Identity.Name);
                UserInfo = new UserInfo()
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    RightDetail = new RightDetail(user.RightLevel)
                };

            }

        }

        protected User GetUser(string userId)
        {
            var cacheKey = UserCachePrefix + userId;
            var user = Cache.Instance.Get(cacheKey) as User;
            if (user == null)
            {
                user = UserService.GetUserById(User.Identity.Name);
                if (user != null)
                {
                    Cache.Instance.Set(cacheKey, user, 60);
                }
            }
            return user;
        }

        protected IEnumerable<User> GetUsers()
        {
            var users = Cache.Instance.Get(UsersCacheKey) as IEnumerable<User>;
            if (users == null)
            {
                users = UserService.GetAllUser();
                Cache.Instance.Set(UsersCacheKey, users, 30);
            }
            return users;
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
                Logger.Warn(filterContext.Exception);
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
