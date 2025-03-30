using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult DashboardAdmin()
        {
            // This will render 'sensor.cshtml' view
            return View();
        }
        public ActionResult sensor()
        {
            // This will render 'sensor.cshtml' view
            return View();
        }
    }
}