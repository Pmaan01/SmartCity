using SmartCity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Services
{
    public interface IWeatherService
    {
        Task<WeatherModel> GetWeatherForCityAsync(string city);
    }
}
