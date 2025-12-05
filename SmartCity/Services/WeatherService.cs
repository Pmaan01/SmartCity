using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SmartCity.Models;

namespace SmartCity.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _http = new();
        private readonly string _apiKey;

        public WeatherService(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("OpenWeatherMap API key must be provided. Set OPENWEATHER_API_KEY in environment or launchSettings.json.", nameof(apiKey));
            _apiKey = apiKey.Trim();
        }

        public async Task<WeatherModel> GetWeatherForCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City must be provided", nameof(city));

            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={Uri.EscapeDataString(city)}&units=metric&cnt=40&appid={_apiKey}";
            using var res = await _http.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenWeather API returned {(int)res.StatusCode} {res.ReasonPhrase}: {content}");
            }

            await using var stream = await res.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            var city_data = root.GetProperty("city");
            var cityName = city_data.GetProperty("name").GetString() ?? city;

            var list = root.GetProperty("list");
            var forecasts = new List<ForecastItem>();

            var processedDates = new HashSet<string>();

            foreach (var item in list.EnumerateArray())
            {
                var dt_txt = item.GetProperty("dt_txt").GetString();
                var dateOnly = dt_txt?.Substring(0, 10);

                if (processedDates.Contains(dateOnly))
                    continue;

                processedDates.Add(dateOnly);

                if (processedDates.Count > 3)
                    break;

                var dt = DateTime.ParseExact(dt_txt, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                var mainData = item.GetProperty("main");
                var weatherArray = item.GetProperty("weather");

                var forecastItem = new ForecastItem
                {
                    Date = dt,
                    Min = mainData.GetProperty("temp_min").GetDouble(),
                    Max = mainData.GetProperty("temp_max").GetDouble(),
                    Condition = weatherArray[0].GetProperty("main").GetString() ?? "Unknown"
                };

                forecasts.Add(forecastItem);
            }

            var currentData = list[0];
            var currentMain = currentData.GetProperty("main");
            var currentWeather = currentData.GetProperty("weather");
            var currentTemp = currentMain.GetProperty("temp").GetDouble();
            var currentCondition = currentWeather[0].GetProperty("main").GetString() ?? "Unknown";
            var currentIcon = currentWeather[0].GetProperty("icon").GetString();

            return new WeatherModel
            {
                City = cityName,
                TempC = currentTemp,
                Condition = currentCondition,
                Icon = currentIcon,
                Forecast = forecasts,
                FetchedAt = DateTime.UtcNow
            };
        }
    }
}