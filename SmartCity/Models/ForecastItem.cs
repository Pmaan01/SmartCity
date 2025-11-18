using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Models
{
    public class ForecastItem
    {
        public DateTime Date { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public string Condition { get; set; }
    }
}
