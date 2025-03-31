using GroupProject.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers.Admin
{
    public class UserManagementController : Controller
    {
        //mysql connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        //Get method to get all users 
        [HttpGet]
        public JsonResult GetUsers(string userType, string searchQuery)
        {
            //create a list to store users
            List<User>users = new List<User>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    //open connection
                    conn.Open();

                    //sql query
                    string query = "SELECT * FROM user WHERE 1=1";

                    //Filter by user type if not "ALL"
                    if (!string.IsNullOrEmpty(userType) && userType != "All")
                    {
                        query += " AND role = @Role";
                    }
                    //Filter by search query
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query += " AND LOWER(name) LIKE LOWER(@SearchQuery)";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        //check for conditions
                        if (userType != "All")
                        {
                            cmd.Parameters.AddWithValue("@Role", userType);
                        }
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                        }
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            //add data to the list
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Role = reader["role"].ToString(),
                                Email = reader["email"].ToString(),
                                Password = reader["password"].ToString(),
                                Status = reader["status"].ToString()
                            });
                        }
                        return Json(users, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //method to check whether 
        [HttpGet]
        public JsonResult CheckUserName(string name)
        {
            //variable to see if the name exists
            bool exists = false;
            try
            {
                using(MySqlConnection  conn = new MySqlConnection(connectionString))
                {
                    //open conn
                    conn.Open();
                    string query  = "SELECT COUNT(*) FROM user WHERE name = @Name";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        exists = count > 0;
                    }
                }

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(exists, JsonRequestBehavior.AllowGet);
        }

        //Post method to create a new admin user
        [HttpPost]
        public JsonResult AddAdmin(string name, string email, string password, string role, string status)
        {

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    //open connection
                    con.Open();
                    // SQL to insert new user
                    string query = "INSERT INTO user (name, email, password, role, status) VALUES (@Name, @Email, @Password, @Role, @Status)";
                    using(MySqlCommand cmd = new MySqlCommand( query, con))
                    {
                        //set data
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Role", role);  
                        cmd.Parameters.AddWithValue("@Status", status);
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false, error = "Failed to create user." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        //user status updated 
        [HttpPost]
        public JsonResult UpdateUserStatus(int userId, string status)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE user SET status = @Status WHERE id = @UserId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Status", status);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Failed to update user status." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}