using System;
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

            var url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&units=metric&appid={_apiKey}";
            using var res = await _http.GetAsync(url);

            if (!res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenWeather API returned {(int)res.StatusCode} {res.ReasonPhrase}: {content}");
            }

            await using var stream = await res.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            var temp = root.GetProperty("main").GetProperty("temp").GetDouble();
            var weatherArray = root.GetProperty("weather");
            var condition = weatherArray[0].GetProperty("main").GetString() ?? "Unknown";
            var icon = weatherArray[0].GetProperty("icon").GetString();

            return new WeatherModel
            {
                City = root.GetProperty("name").GetString() ?? city,
                TempC = temp,
                Condition = condition,
                Icon = icon,
                FetchedAt = DateTime.UtcNow
            };
        }
    }
}