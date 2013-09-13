using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    public class ProjectController : BaseController
    {
        //
        // GET: /Project/
        [HttpGet]
        public ActionResult List()
        {
            //ViewBag.User
            //this.User.Identity.AuthenticationType
            return View();
        }

    }
}
