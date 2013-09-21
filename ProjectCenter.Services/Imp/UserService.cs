using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ProjectCenter.Util.Exceptions;

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
        public static IEnumerable<User> _usersCache;
        public static IDictionary<string, User> _usersDicByLoginName = new Dictionary<string, User>();
        public static IDictionary<string, User> _usersDicById = new Dictionary<string, User>();

        public UserService(DbContext dbContext)
            : base(dbContext)
        {
        }

        public static void InitializeCache(DbContext dbContext)
        {
            _usersCache = dbContext.Set<User>().OrderBy(u => u.RightLevel).ToArray();
            foreach (var user in _usersCache)
            {
                _usersDicByLoginName.Add(user.LoginName, user);
                _usersDicById.Add(user.Id, user);
            }
        }

        public User GetUserById(string userId)
        {
            User user = null;
            _usersDicById.TryGetValue(userId, out user);
            return user;
        }

        public User Login(string loginName, string password)
        {
            User user = null;
            if (!_usersDicByLoginName.TryGetValue(loginName, out user))
            {
                throw new BusinessException("不存在该登录名的用户");
            }
            return user.Passwrod == password ? user : null;
            //return GetMD5String(password) == user.Passwrod;
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
            return _usersCache;
        }
    }
}
