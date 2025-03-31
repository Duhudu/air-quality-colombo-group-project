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
                    string query = "INSERT INTO sensor (location, sensorStatus, latitude, longitude, name) VALUES (@Location, @Status, @Latitude, @Longitude, @Name)";
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

                    // Delete records from airqualityhistory where sensor_id matches the one being deleted
                    string deleteHistoryQuery = "DELETE FROM airqualityhistory WHERE sensor_id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(deleteHistoryQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        // Delete related records in airqualityhistory
                        cmd.ExecuteNonQuery();  
                    }

                    // delete the sensor from the Sensor table
                    string deleteSensorQuery = "DELETE FROM Sensor WHERE id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(deleteSensorQuery, conn))
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

        //function to see if all sensors active 
        [HttpGet]
        public JsonResult GetSensorStatus(bool checkForActive = false)
        {
            List<SensorStatusModel> sensors = new List<SensorStatusModel>();
            int statusCount = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT sensorStatus FROM Sensor";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var sensorStatus = new SensorStatusModel
                                {
                                    Status = reader["sensorStatus"].ToString()
                                };

                                sensors.Add(sensorStatus);

                                // Count how many sensors are 'active' or 'inactive' based on checkForActive flag
                                if (checkForActive)
                                {
                                    if (sensorStatus.Status.ToLower() == "active")
                                    {
                                        statusCount++;
                                    }
                                }
                                else
                                {
                                    if (sensorStatus.Status.ToLower() == "inactive")
                                    {
                                        statusCount++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            // Check if all sensors are inactive
            bool allInactive = !checkForActive && statusCount == sensors.Count;

            // Check if all sensors are active
            bool allActive = checkForActive && statusCount == sensors.Count;

            if (allInactive)
            {
                return Json(new { success = true, message = "All sensors are inactive.", allInactive = true }, JsonRequestBehavior.AllowGet);
            }

            if (allActive)
            {
                return Json(new { success = true, message = "All sensors are active.", allActive = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = $"There are {statusCount} sensors in the desired status." }, JsonRequestBehavior.AllowGet);
        }

        //methods to set sensros to active and inactive
        [HttpPost]
        public JsonResult SetSensorsStatusInactive()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Sensor SET sensorStatus = 'inactive'";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SetSensorsStatusActive()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Sensor SET sensorStatus = 'active'";
                    using(MySqlCommand cmd = new MySqlCommand( query, conn))
                    {
                        cmd.ExecuteNonQuery();
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