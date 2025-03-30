using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject.Models
{
    public class SensorAirQualityData
    {
        public int SensorId { get; set; }
        public string SensorName { get; set; }
        public string SensorLocation { get; set; }
        public string SensorStatus { get; set; }
        public float SensorLatitude { get; set; }
        public float SensorLongitude { get; set; }
        public float PM2_5 { get; set; }
        public float PM10 { get; set; }
        public float Pm1 { get; set; }
        public float RH { get; set; }
        public float Temp { get; set; }
        public float Wind { get; set; }
        public int Aqi { get; set; }
        public DateTime? ReadingDate { get; set; }
    }
}