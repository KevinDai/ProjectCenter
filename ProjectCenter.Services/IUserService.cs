using ProjectCenter.Models;
using ProjectCenter.Services.Models;
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

        LoginResult Login(string loginName, string password);
    }
}
