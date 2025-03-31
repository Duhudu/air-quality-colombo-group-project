using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult CreateAccount()
        {
            return View();
        }

        // Display profile if the user is logged in
        public ActionResult Profile()
        {
            //// If user is logged in (session exists)
            if (Session["UserName"] != null)
            {
                // Retrieve user details from the session
                string userName = Session["UserName"].ToString();
                string userEmail = Session["UserEmail"]?.ToString();

                // Send the data to the view
                ViewBag.UserName = userName;
                ViewBag.UserEmail = userEmail;

                return View();
            }
            else
            {
                // If not logged in, redirect to login page
                return RedirectToAction("Login", "LogAccount");
            }
            //return View();
        }

        // Logout action to clear session
        public ActionResult Logout()
        {
            Session.Clear(); // Clear all session data
            return RedirectToAction("Login", "Account");
        }

    }


}
