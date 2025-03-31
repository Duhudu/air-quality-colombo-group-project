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
        public ActionResult userManagement()
        {
            // This will render 'sensor.cshtml' view
            return View();
        }

        public ActionResult Profile()
        {
            // If user is logged in (session exists)
            //if (Session["UserName"] != null)
            //{
            //    // Retrieve user details from the session
            //    string userName = Session["UserName"].ToString();
            //    string userEmail = Session["UserEmail"]?.ToString();

            //    // Send the data to the view
            //    ViewBag.UserName = userName;
            //    ViewBag.UserEmail = userEmail;

            //    return View();
            //}
            //else
            //{
            //    // If not logged in, redirect to login page
            //    return RedirectToAction("Login", "Account");
            //}
            return View();
        }

    }
}