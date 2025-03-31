using GroupProject.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers
{
    public class LogAccountController : Controller
    {
        // MySQL connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        // Login GET: Show login form
        public ActionResult Login()
        {
            return View();
        }

        // Login POST: Validate user credentials and redirect based on role and status
        [HttpPost]
        public ActionResult Login(string Name, string Password)
        {
            User user = AuthenticateUser(Name, Password);

            if (user == null)
            {
                // If no user is found or authentication fails
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View("~/Views/Home/LogIn.cshtml");
            }

            // Check if user status is "Enter"
            if (user.Status != "Enter")
            {
                // If status is not "Enter", show a message that they are banned
                ViewBag.ErrorMessage = "You are banned from the system.";
                return View();
            }

            // Set session variables for user details
            Session["UserId"] = user.Id;
            Session["UserName"] = user.Name;
            Session["UserRole"] = user.Role;

            // Redirect based on user role
            if (user.Role == "SuperAdmin" || user.Role == "Admin")
            {
                return RedirectToAction("DashboardAdmin", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // Authenticate user by checking username and password in the database
        private User AuthenticateUser(string name, string password)
        {
            User user = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM User WHERE name = @Name";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string storedPassword = reader["password"].ToString();
                            // If the password matches, create user object
                            if (BCrypt.Net.BCrypt.Verify(password, storedPassword)) // assuming you are using hashed passwords
                            {
                                user = new User
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString(),
                                    Role = reader["role"].ToString(),
                                    Status = reader["status"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., log them)
                Console.WriteLine(ex.Message);
            }
            return user;
        }

        // Logout action to clear session
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "LogAccount");
        }
    }
}