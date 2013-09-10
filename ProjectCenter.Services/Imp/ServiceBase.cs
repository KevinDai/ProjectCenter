using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data;

namespace ProjectCenter.Services.Imp
{
    internal abstract class ServiceBase
    {
        protected DbContext DbContext
        {
            get;
            private set;
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

        public virtual void RemoveEntity<T, TId>(T entity) where T : class
        {
            DbContext.Entry<T>(entity).State = EntityState.Deleted;
        }

        public virtual void SaveChanges()
        {
            DbContext.SaveChanges();
        }

    }
}
