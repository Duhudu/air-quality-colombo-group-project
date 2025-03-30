using GroupProject.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers.Admin
{
    public class SensorController : Controller
    {
        ////mysql connection string
        //private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        //// GET: /Sensor/GetSensors
        //[HttpGet]
        //public JsonResult GetSensors(string searchQuery)
        //{
        //    List<Sensor> sensors = new List<Sensor>();
        //    try
        //    {
        //        //GetSensors  mehtod
        //        using (MySqlConnection conn = new MySqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            // Start the base query
        //            string query = "SELECT * FROM Sensor WHERE 1=1";

        //            // Filter by search query (location)
        //            if (!string.IsNullOrEmpty(searchQuery))
        //            {
        //                query += " AND location LIKE @SearchQuery";
        //            }

        //            // Create the MySQL command with the query
        //            using (MySqlCommand cmd = new MySqlCommand(query, conn))
        //            {
        //                // Only add the parameter if the search query is provided
        //                if (!string.IsNullOrEmpty(searchQuery))
        //                {
        //                    cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
        //                }

        //                // Execute the query and read the results
        //                MySqlDataReader reader = cmd.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    sensors.Add(new Sensor
        //                    {
        //                        Id = Convert.ToInt32(reader["id"]),
        //                        Name = reader["name"].ToString(),
        //                        Location = reader["location"].ToString(),
        //                        Status = reader["sensorStatus"].ToString(),
        //                        Latitude = reader.IsDBNull(reader.GetOrdinal("latitude")) ? 0f : (float)Convert.ToDouble(reader["latitude"]),
        //                        Longitude = reader.IsDBNull(reader.GetOrdinal("longitude")) ? 0f : (float)Convert.ToDouble(reader["longitude"]),


        //                    });
        //                }
        //            }
        //        }
        //        return Json(sensors, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        ////Add Sensor Method
        //[HttpPost]
        //public JsonResult AddSensor(Sensor sensor)
        //{
        //    try
        //    {
        //        using (MySqlConnection con = new MySqlConnection(connectionString))
        //        {
        //            // Open connection
        //            con.Open();

        //            // SQL query to insert sensor data
        //            string query = "INSERT INTO Sensor (name, location, sensorStatus, latitude, longitude) VALUES (@Name, @Location, @Status, @Latitude, @Longitude)";

        //            using (MySqlCommand cmd = new MySqlCommand(query, con))
        //            {

        //                cmd.Parameters.AddWithValue("@Name", sensor.Name);
        //                cmd.Parameters.AddWithValue("@Location", sensor.Location);
        //                cmd.Parameters.AddWithValue("@Status", sensor.Status);
        //                cmd.Parameters.AddWithValue("@Latitude", sensor.Latitude);
        //                cmd.Parameters.AddWithValue("@Longitude", sensor.Longitude);

        //                // Execute the query
        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //        return Json(new { success = true, message = "Sensor added successfully!" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for more details
        //        Console.WriteLine(ex.Message);
        //        return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //mysql connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        // GET: /Sensor/GetSensors
        [HttpGet]
        public JsonResult GetSensors(string searchQuery)
        {
            List<Sensor> sensors = new List<Sensor>();
            try
            {
                //GetSensors  mehtod
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Start the base query
                    string query = "SELECT * FROM Sensor WHERE 1=1";

                    // Filter by search query (location)
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query += " AND location LIKE @SearchQuery";
                    }

                    // Create the MySQL command with the query
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Only add the parameter if the search query is provided
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                        }

                        // Execute the query and read the results
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            sensors.Add(new Sensor
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Location = reader["location"].ToString(),
                                Status = reader["sensorStatus"].ToString(),
                                Latitude = reader.IsDBNull(reader.GetOrdinal("latitude")) ? 0f : (float)Convert.ToDouble(reader["latitude"]),
                                Longitude = reader.IsDBNull(reader.GetOrdinal("longitude")) ? 0f : (float)Convert.ToDouble(reader["longitude"]),
                                

                            });
                        }
                    }
                }
                return Json(sensors, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }





        [HttpPost]
        public JsonResult AddSensor(Sensor sensor)
        {
            try
            {
                
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO sensor (location, sensorStatus, latitude, longitude) VALUES (@Location, @Status, @Latitude, @Longitude)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", sensor.Name);
                        cmd.Parameters.AddWithValue("@Location", sensor.Location);
                        cmd.Parameters.AddWithValue("@Status", sensor.Status);
                        cmd.Parameters.AddWithValue("@Latitude", sensor.Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", sensor.Longitude);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                return Json(new { success = true, message = "Sensor added successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // POST: /Sensor/UpdateSensor
        [HttpPost]
        public JsonResult UpdateSensor(Sensor updatedSensor)
        {
            //UpdateSensor method
            try
            {

                // Log the incoming sensor data for debugging
                System.Diagnostics.Debug.WriteLine($"Updating sensor with ID: {updatedSensor.Id}, Name: {updatedSensor.Name}, Location: {updatedSensor.Location}, Status: {updatedSensor.Status}");

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // SQL query to update the sensor
                    string query = "UPDATE sensor SET name = @Name, location = @Location, sensorStatus = @Status, latitude = @Latitude, longitude = @Longitude WHERE id = @Id";
                    Console.WriteLine("Updating sensor with ID: " + updatedSensor.Id);
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        cmd.Parameters.AddWithValue("@Id", updatedSensor.Id);
                        cmd.Parameters.AddWithValue("@Name", updatedSensor.Name);
                        cmd.Parameters.AddWithValue("@Location", updatedSensor.Location);
                        cmd.Parameters.AddWithValue("@Status", updatedSensor.Status);
                        cmd.Parameters.AddWithValue("@Latitude", updatedSensor.Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", updatedSensor.Longitude);


                        // Execute the query and log the number of affected rows
                        int rowsAffected = cmd.ExecuteNonQuery();
                        // Log how many rows are updated
                        System.Diagnostics.Debug.WriteLine($"Rows affected: {rowsAffected}");
                    }
                }
                // Return success if update is successful
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Return error message if an exception occurs
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // POST: /Sensor/DeleteSensor
        [HttpPost]
        public JsonResult DeleteSensor(int id)
        {
            //Delete Sensor Method
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Set sensor_id to NULL in airqualityhistory where sensor_id is the one being deleted
                    string updateQuery = "UPDATE airqualityhistory SET sensor_id = NULL WHERE sensor_id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();  // Set sensor_id to NULL for the deleted sensor
                    }

                    // Now delete the sensor from the Sensor table
                    string deleteQuery = "DELETE FROM Sensor WHERE id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();  // Delete the sensor
                    }
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}