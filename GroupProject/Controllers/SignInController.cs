using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers
{
    public class SignInController : Controller
    {
        //mysql connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        [HttpPost]
        public JsonResult AddUser(string name, string email, string password, string role, string status)
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
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
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

        //method to check whether 
        [HttpGet]
        public JsonResult CheckUserName(string name)
        {
            //variable to see if the name exists
            bool exists = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    //open conn
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM user WHERE name = @Name";
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



    }


}