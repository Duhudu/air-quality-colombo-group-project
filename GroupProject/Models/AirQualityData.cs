using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject.Models
{
    public class AirQualityData
    {
        public int SensorId { get; set; }
        public float PM2_5 { get; set; }
        public float PM10 { get; set; }
        public float Pm1 { get; set; }
        public float RH { get; set; }
        public float Temp { get; set; }
        public float Wind { get; set; }
        public int Aqi { get; set; }
    }
}