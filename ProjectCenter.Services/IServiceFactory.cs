using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Services
{
    public interface IServiceFactory
    {
        IProjectService CreateProjectService();

    }
}
