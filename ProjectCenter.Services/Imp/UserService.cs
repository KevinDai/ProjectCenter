using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Services.Models;

namespace ProjectCenter.Services.Imp
{
    internal class UserService : ServiceBase, IUserService
    {

        protected IDbSet<User> Users
        {
            get
            {
                return DbContext.Set<User>();
            }
        }
        //public static IEnumerable<User> _usersCache;
        //public static IDictionary<string, User> _usersDicByLoginName = new Dictionary<string, User>();
        //public static IDictionary<string, User> _usersDicById = new Dictionary<string, User>();

        public UserService(DbContext dbContext)
            : base(dbContext)
        {
        }

        //public static void InitializeCache(DbContext dbContext)
        //{
        //    _usersCache = dbContext.Set<User>().OrderBy(u => u.RightLevel).ToArray();
        //    foreach (var user in _usersCache)
        //    {
        //        _usersDicByLoginName.Add(user.LoginName, user);
        //        _usersDicById.Add(user.Id, user);
        //    }
        //}

        public User GetUserById(string userId)
        {

            return Users.Find(userId);
            //User user = null;
            //_usersDicById.TryGetValue(userId, out user);
            //return user;
        }

        public LoginResult Login(string loginName, string password)
        {
            LoginResult result = new LoginResult();

            User user = Users.FirstOrDefault(u => u.LoginName == loginName);
            if (user == null)
            {
                result.FailMessage = "不存在该用户名的用户";
            }
            else if (user.RightLevel < 0)
            {
                result.FailMessage = "该用户不允许登录";
            }
            else if (GetMD5String(password) != user.Passwrod)
            {
                result.FailMessage = "用户名或者密码错误";
            }
            else
            {
                result.User = user;
            }
            return result;
        }

        private string GetMD5String(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] b = Encoding.UTF8.GetBytes(str);
            byte[] md5b = md5.ComputeHash(b);
            md5.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (var item in md5b)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }

        public IEnumerable<User> GetAllUser()
        {
            return Users.OrderBy(u => u.RightLevel).ToArray();
        }
    }
}
