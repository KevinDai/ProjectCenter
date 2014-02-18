using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ProjectCenter.Util.Exceptions;
using ProjectCenter.Services.Models;
using ProjectCenter.Util.Query;
using ProjectCenter.Util.Query.Specification;
using ProjectCenter.Util.Query.Extensions;
using System.Transactions;

namespace ProjectCenter.Services.Imp
{
    internal class UserService : ServiceBase, IUserService
    {

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
            else if (user.RightLevel <= 0)
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

        public void ChangePassword(string userId, string password)
        {
            User user = Users.FirstOrDefault(u => u.Id == userId);
            user.Passwrod = GetMD5String(password);
            UpdateEntity(user);
            SaveChanges();
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

        public IEnumerable<User> GetUserList(ISpecification<User> spec, SortDescriptor<User>[] sorts)
        {
            var query = Users.FindBy(spec);
            var result = query.Sort(sorts);
            return result.ToArray();
        }

        public void EditUserBaseInfo(User user)
        {
            User entity = Users.FirstOrDefault(u => u.Id == user.Id);

            entity.Name = user.Name;
            entity.RightLevel = user.RightLevel;

            UpdateEntity(entity);
            SaveChanges();
        }

        public void AddUser(User user)
        {
            using (var ts = new TransactionScope())
            {
                if (Users.Count(u => u.LoginName == user.LoginName) > 0)
                {
                    throw new BusinessException("已存在指定登录名称的用户");
                }

                user.Passwrod = GetMD5String(Constants.DefaultPassword);
                user.Id = Guid.NewGuid().ToString();

                AddEntity(user);

                SaveChanges();
                ts.Complete();
            }

        }

        public void DeleteUser(string userId)
        {
            User entity = Users.FirstOrDefault(u => u.Id == userId);
            if (entity != null)
            {
                DeleteEntity(entity);
                SaveChanges();
            }
        }
    }
}
