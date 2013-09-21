using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectCenter.Services.Imp;
using ProjectCenter.Models;
using System.Data.Entity;

namespace ProjectCenter.Services
{
    public class ServiceFactory : IServiceFactory
    {
        private static IServiceFactory _instance = new ServiceFactory();

        public static IServiceFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        protected ServiceFactory()
        {
            UserService.InitializeCache(GetDbContext());
        }

        protected virtual DbContext GetDbContext()
        {
            return new ProjectCenterContext();
        }

        public IProjectService CreateProjectService()
        {
            return new ProjectService(GetDbContext());
        }

        public IUserService CreateUserService()
        {
            return new UserService(GetDbContext());
        }
    }
}
