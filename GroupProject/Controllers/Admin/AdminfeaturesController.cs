using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers.Admin
{
    public class AdminfeaturesController : Controller
    {
        //mysql connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        //Get method DashBoardCount
        [HttpGet]
        public JsonResult DashBoardCount()
        {
            //variable to store the counts 
            int activeSensors = 0;
            int inActiveSensors = 0;
            int totalUsers = 0;
            int adminUsers = 0;

            try
            {
                using(MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    //open conn
                    conn.Open();

                    //Get the number of Active Sensors
                    string activeSensorQuery = "SELECT COUNT(*) FROM sensor WHERE sensorStatus = 'Active'";
                    using (MySqlCommand cmd = new MySqlCommand(activeSensorQuery, conn))
                    {
                        //Execute and get the active sensor count
                        activeSensors = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    //Get the number of Inactive Sensors
                    string inActiveSensorQuery = "SELECT COUNT(*) FROM sensor WHERE sensorStatus = 'Inactive'";
                    using (MySqlCommand cmd = new MySqlCommand(inActiveSensorQuery, conn))
                    {
                        //Execute and get the inactive sensors count
                        inActiveSensors = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    //Get the number of Registered Users
                    string totalUsersQuery = "SELECT COUNT(*) FROM user WHERE role ='User'";
                    using (MySqlCommand cmd = new MySqlCommand(totalUsersQuery, conn))
                    {
                        //Execute and get the User count
                        totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    //Get the number of Admin accs 
                    string adminUsersQuery = "SELECT COUNT(*) FROM user WHERE role ='Admin'";
                    using (MySqlCommand cmd = new MySqlCommand(adminUsersQuery, conn))
                    {
                        //Execute and get the number of admin accs
                        adminUsers = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the database operation
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            //Prepare a model with the fetched data 
            var dashboardData = new
            {
                ActiveSensors = activeSensors,
                InActiveSensors = inActiveSensors,
                TotalUsers = totalUsers,
                AdminUsers = adminUsers
            };

            //Return the data to the view
            return Json(dashboardData, JsonRequestBehavior.AllowGet);
        }
    }
}