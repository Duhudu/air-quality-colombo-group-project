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
    public class DynamicDataController : Controller
    {
        //mysql connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        //Get method to take the air quality data
        [HttpGet]
        public JsonResult GetAirQuality(int? sensorId, DateTime? dateFrom, DateTime? dateTo)
        {
            List<AirQualityData> airQualityDataList = new List<AirQualityData>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Base query
                    string query = @"
                        SELECT 
                            s.id AS SensorId,
                            s.location AS SensorLocation,
                            s.sensorStatus AS SensorStatus,
                            s.latitude AS SensorLatitude,
                            s.longitude AS SensorLongitude,
                            a.PM25,
                            a.PM10,
                            a.PM1,
                            a.RH,
                            a.Temp,
                            a.Wind,
                            a.Aqi,
                            a.date AS ReadingDate
                        FROM 
                            Sensor s
                        JOIN 
                            airqualityhistory a ON s.id = a.sensor_id
                        WHERE 1=1 ";

                    // Add filter conditions dynamically
                    if (sensorId.HasValue)
                    {
                        query += " AND s.id = @sensorId";
                    }
                    if (dateFrom.HasValue)
                    {
                        query += " AND a.date >= @dateFrom";
                    }
                    if (dateTo.HasValue)
                    {
                        query += " AND a.date <= @dateTo";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Add parameters dynamically based on the filters
                        if (sensorId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@sensorId", sensorId.Value);
                        }
                        if (dateFrom.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@dateFrom", dateFrom.Value);
                        }
                        if (dateTo.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@dateTo", dateTo.Value);
                        }

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            airQualityDataList.Add(new AirQualityData
                            {
                                SensorId = reader.GetInt32("SensorId"),
                                PM2_5 = reader.GetFloat("PM25"),
                                PM10 = reader.GetFloat("PM10"),
                                Pm1 = reader.GetFloat("PM1"),
                                RH = reader.GetFloat("RH"),
                                Temp = reader.GetFloat("Temp"),
                                Wind = reader.GetFloat("Wind"),
                                Aqi = reader.GetInt32("Aqi")
                            });
                        }
                    }
                }

                return Json(airQualityDataList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}