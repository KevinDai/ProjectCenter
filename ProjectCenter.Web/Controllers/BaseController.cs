using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    [HandleError(View = "Error", ExceptionType = typeof(Exception))]
    public abstract class BaseController : Controller
    {
        protected ILog Logger = LogManager.GetLogger("");

    }
}