using GroupProject.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace GroupProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Timer _timer;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Start the timer to run the task every 10 minutes
            StartBackgroundTask();
        }
        private void StartBackgroundTask()
        {
            // Execute the task immediately
            ExecuteBackgroundTask(null);

            // Now set the timer to execute the background task every 10 minutes
            _timer = new Timer(ExecuteBackgroundTask, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        }
        // This is the method that will be called by the timer every 10 minutes
        private void ExecuteBackgroundTask(object state)
        {
            try
            {
                // background task logic here
                Console.WriteLine($"Background task started at: {DateTime.Now}");
                // Get the list of sensors from the database
                var sensors = GetSensorsFromDatabase();


                // Loop through the sensors and process each one
                foreach (var sensor in sensors)
                {
                    
                    //// Insert the generated data into the airqualityhistory table with the next timestamp
                    //InsertDataIntoDatabase(generatedData, nextTimestamp);
                    // Check if the sensor status is "active" before processing it
                    if (sensor.Status.ToLower() == "active")  // Status is "active", proceed
                    {
                        // Get the last data entry timestamp for the sensor
                        DateTime lastTimestamp = GetLastTimestampForSensor(sensor.Id);

                        // Calculate the next timestamp, which should be 10 minutes after the last one
                        DateTime nextTimestamp = lastTimestamp.AddMinutes(10);

                        // Generate new air quality data for the sensor
                        var generatedData = GenerateAirQualityData(sensor.Id);

                        // Insert the generated data into the airqualityhistory table with the next timestamp
                        InsertDataIntoDatabase(generatedData, nextTimestamp);
                    }
                    else
                    {
                        // **If the sensor status is not "active", skip it and print a message** (optional)
                        Console.WriteLine($"Skipping sensor {sensor.Name} as it is not active.");
                    }
                }

                Console.WriteLine($"Background task completed at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing background task: {ex.Message}");
            }
        }
        //Method to generate air quality data for a given sensor
        private Random _random = new Random();
        private AirQualityData GenerateAirQualityData(int sensorId)
        {
            return new AirQualityData
            {
                SensorId = sensorId,
                PM2_5 = _random.Next(0, 100),
                PM10 = _random.Next(0, 100),
                Pm1 = _random.Next(0, 100),
                RH = _random.Next(0, 100),
                Temp = _random.Next(-10, 50),
                Wind = _random.Next(0, 100),
                Aqi = _random.Next(0, 400)
            };
            
        }

        // Method to get the last timestamp from the airqualityhistory table for a given sensor
        private DateTime GetLastTimestampForSensor(int sensorId)
        {
            //sql connection
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using(MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                // Query to get the most recent timestamp for the sensor
                string query = "SELECT MAX(date) FROM airqualityhistory WHERE sensor_id = @SensorId";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SensorId", sensorId);

                    // Execute the query and return the result
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToDateTime(result); // If data exists, return the last timestamp
                    }
                    else
                    {
                        return DateTime.Now.AddMinutes(-10); // If no data exists, return a default timestamp
                    }
                }
            }
        }

        protected void Application_End()
        {
            // Make sure to dispose of the timer when the application ends
            _timer?.Dispose();
        }
        public void GenerateAndInsertData()
        {
            //Get the sensor data from the database (fetching sensor IDs and other necessary details)
            var sensors = GetSensorsFromDatabase();

        }
        public List<Sensor> GetSensorsFromDatabase()
        {
            var sensors = new List<Sensor>();
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query to fetch sensor details from the sensor table
                string query = "SELECT * FROM Sensor";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
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
            }
            return sensors;
        }

        // Method to insert the generated data into the airqualityhistory table
        public void InsertDataIntoDatabase(AirQualityData data, DateTime timestamp)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO airqualityhistory (Date, PM25, PM10, PM1, RH, Temp, Wind, aqi, sensor_id)
                    VALUES (@Date, @PM25, @PM10, @PM1, @RH, @Temp, @Wind, @Aqi, @SensorId)";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SensorId", data.SensorId);
                    cmd.Parameters.AddWithValue("@Date", timestamp);
                    cmd.Parameters.AddWithValue("@PM25", data.PM2_5);
                    cmd.Parameters.AddWithValue("@PM10", data.PM10);
                    cmd.Parameters.AddWithValue("@PM1", data.Pm1);
                    cmd.Parameters.AddWithValue("@RH", data.RH);
                    cmd.Parameters.AddWithValue("@Temp", data.Temp);
                    cmd.Parameters.AddWithValue("@Wind", data.Wind);
                    cmd.Parameters.AddWithValue("@Aqi", data.Aqi);
                    

                    // Execute the insert query
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
