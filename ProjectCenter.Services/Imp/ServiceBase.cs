using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data;
using ProjectCenter.Models;

namespace ProjectCenter.Services.Imp
{
    internal abstract class ServiceBase
    {
        protected DbContext DbContext
        {
            get;
            private set;
        }

        protected IDbSet<User> Users
        {
            get
            {
                return DbContext.Set<User>();
            }
        }

        public ServiceBase(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual void AddEntity<T>(T entity) where T : class
        {
            DbContext.Entry<T>(entity).State = EntityState.Added;
        }

        public virtual void UpdateEntity<T>(T entity) where T : class
        {
            DbContext.Entry<T>(entity).State = EntityState.Modified;
        }

        public virtual void DeleteEntity<T>(T entity) where T : class
        {
            DbContext.Entry<T>(entity).State = EntityState.Deleted;
        }

        public virtual void SaveChanges()
        {
            DbContext.SaveChanges();
        }

    }
}
