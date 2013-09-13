using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.Controllers
{
    public class NoteController : BaseController
    {
        //
        // GET: /Note/

        public ActionResult List()
        {
            return View();
        }

    }
}
