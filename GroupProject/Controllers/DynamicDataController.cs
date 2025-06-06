﻿using GroupProject.Models;
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
            List<SensorAirQualityData> sensorAirQualityDataList = new List<SensorAirQualityData>();


            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Base query
                    string query = @"
                    SELECT 
                        s.id AS SensorId,
                        s.name AS SensorName, 
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
                    LEFT JOIN 
                        airqualityhistory a ON s.id = a.sensor_id ";





                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {


                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            sensorAirQualityDataList.Add(new SensorAirQualityData
                            {
                                SensorId = reader.GetInt32("SensorId"),
                                SensorName = reader.GetString("SensorName"),
                                SensorLocation = reader.GetString("SensorLocation"),
                                SensorStatus = reader.GetString("SensorStatus"),
                                SensorLatitude = reader.IsDBNull(reader.GetOrdinal("SensorLatitude")) ? 0 : reader.GetFloat("SensorLatitude"),
                                SensorLongitude = reader.IsDBNull(reader.GetOrdinal("SensorLongitude")) ? 0 : reader.GetFloat("SensorLongitude"),
                                PM2_5 = reader.IsDBNull(reader.GetOrdinal("PM25")) ? 0 : reader.GetFloat("PM25"),
                                PM10 = reader.IsDBNull(reader.GetOrdinal("PM10")) ? 0 : reader.GetFloat("PM10"),
                                Pm1 = reader.IsDBNull(reader.GetOrdinal("PM1")) ? 0 : reader.GetFloat("PM1"),
                                RH = reader.IsDBNull(reader.GetOrdinal("RH")) ? 0 : reader.GetFloat("RH"),
                                Temp = reader.IsDBNull(reader.GetOrdinal("Temp")) ? 0 : reader.GetFloat("Temp"),
                                Wind = reader.IsDBNull(reader.GetOrdinal("Wind")) ? 0 : reader.GetFloat("Wind"),
                                Aqi = reader.IsDBNull(reader.GetOrdinal("Aqi")) ? 0 : reader.GetInt32("Aqi"),
                                ReadingDate = reader.IsDBNull(reader.GetOrdinal("ReadingDate")) ? (DateTime?)null : reader.GetDateTime("ReadingDate")
                            });
                        }
                    }
                }

                return Json(sensorAirQualityDataList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}