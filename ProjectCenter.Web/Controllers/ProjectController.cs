﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    public class ProjectController : Controller
    {
        //
        // GET: /Project/
        [HttpGet]
        public ActionResult List()
        {
            //this.User.Identity.AuthenticationType
            return View();
        }

    }
}
