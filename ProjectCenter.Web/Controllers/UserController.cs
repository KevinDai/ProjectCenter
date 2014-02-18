using ProjectCenter.Models;
using ProjectCenter.Services;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;
using ProjectCenter.Web.Models;
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

        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult LoadUsers()
        {
            var users = UserService.GetUserList(
                 new DirectSpecification<User>(u => u.RightLevel >= 0),
                 new SortDescriptor<User>[] { SortDescriptor<User>.CreateSortDescriptor(u => u.RightLevel) });
            return JsonMessageResult(users.Select(u => new UserListItem(u)).ToArray());
        }

        public ActionResult AddUser(User user)
        {
            UserService.AddUser(user);

            return JsonMessageResult(null);
        }

        public ActionResult EditUserBaseInfo(User user)
        {
            UserService.EditUserBaseInfo(user);

            return JsonMessageResult(null);
        }

        public ActionResult DeleteUser(string userId)
        {
            UserService.DeleteUser(userId);
            return JsonMessageResult(null);
        }

        public ActionResult ResetPassword(string userId)
        {
            UserService.ChangePassword(userId, Constants.DefaultPassword);
            return JsonMessageResult(null);
        }

    }
}