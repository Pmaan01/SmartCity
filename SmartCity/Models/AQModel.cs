using System;
using System.Collections.Generic;

namespace SmartCity.Models
{
    public class AQModel
    {
        public string City { get; set; } = "Unknown";
        public int AQI { get; set; }
        public string Category { get; set; } = "Unknown";
        public string MainPollutant { get; set; } = "N/A";
        public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
        public IList<int> HourlyValues { get; set; } = new List<int>();
    }
}