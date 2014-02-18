using ProjectCenter.Models;
using ProjectCenter.Services.Models;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Services
{
    public interface IUserService
    {
        User GetUserById(string userId);

        IEnumerable<User> GetAllUser();

        IEnumerable<User> GetUserList(ISpecification<User> spec, SortDescriptor<User>[] sorts);

        void EditUserBaseInfo(User user);

        void AddUser(User user);

        void DeleteUser(string userId);

        LoginResult Login(string loginName, string password);

        void ChangePassword(string userId, string password);
    }
}
