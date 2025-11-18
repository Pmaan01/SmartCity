using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Models
{
    public class WeatherModel
    {
        public string City { get; set; }
        public double TempC { get; set; }
        public string Condition { get; set; }
        public string Icon { get; set; }
        public List<ForecastItem> Forecast { get; set; } = new();
        public DateTime FetchedAt { get; set; }
    }
}
