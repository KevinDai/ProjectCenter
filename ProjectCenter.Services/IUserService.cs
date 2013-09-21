using ProjectCenter.Models;
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

        User Login(string loginName, string password);
    }
}
