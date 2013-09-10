using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace ProjectCenter.Services.Imp
{
    internal class ProjectService : ServiceBase, IProjectService
    {
        public ProjectService(DbContext dbContext)
            : base(dbContext)
        {
        }


    }
}
