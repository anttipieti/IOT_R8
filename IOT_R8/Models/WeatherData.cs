using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class WeatherData
    {
        public long Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Light { get; set; }
    }
}
