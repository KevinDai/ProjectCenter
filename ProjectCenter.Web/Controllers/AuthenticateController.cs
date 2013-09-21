using ProjectCenter.Services;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjectCenter.Web.Controllers
{
    public class AuthenticateController : Controller
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

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return new RedirectResult("/");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, string password, string returnUrl)
        {
            try
            {
                var user = UserService.Login(name, password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Id, true);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return new RedirectResult(returnUrl);
                    }
                    else
                    {
                        return new RedirectResult("/");
                    }
                }
                return View(new LoginViewModel() { LoginName = name, Message = "" });
            }
            catch (BusinessException ex)
            {
                return View(new LoginViewModel() { LoginName = name, Message = ex.Message });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return new RedirectResult(FormsAuthentication.LoginUrl);
        }

    }
}
