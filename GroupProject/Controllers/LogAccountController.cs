using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers
{
	public class LogAccountController : Controller
	{
		private readonly string _connectionString = "Server=localhost;Database=airQuality;User=root;Password=PHW#84#jeor;";

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(string email, string password)
		{
			using (var connection = new MySqlConnection(_connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM User WHERE email = @Email AND password = @Password";
				var command = new MySqlCommand(query, connection);
				command.Parameters.AddWithValue("@Email", email);
				command.Parameters.AddWithValue("@Password", password); // In a real app, you should hash the password

				var reader = command.ExecuteReader();
				if (reader.Read())
				{
					var user = new User
					{
						Id = reader.GetInt32("id"),
						Name = reader.GetString("name"),
						Role = reader.GetString("role"),
						Email = reader.GetString("email"),
						CreatedAt = reader.GetDateTime("created_at"),
						UpdatedAt = reader.GetDateTime("updated_at")
					};

					// Store the user in session or authentication token
					HttpContext.Current.Session["UserId"] = user.Id.ToString();

					HttpContext.Session.SetString("UserName", user.Name);

					return RedirectToAction("Index", "Home"); // Redirect to home page or dashboard after login
				}
				else
				{
					ViewBag.Error = "Invalid email or password!";
				}
			}

			return View();
		}
	}
}