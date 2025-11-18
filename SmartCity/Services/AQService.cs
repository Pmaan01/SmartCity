using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SmartCity.Models;

namespace SmartCity.Services
{
    public class AQService : IAQService
    {
        private readonly HttpClient _http = new();
        private readonly string? _apiKey;

        public AQService() => _apiKey = null;
        public AQService(string? apiKey) => _apiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;

        public async Task<AQModel> GetAqForLocationAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                await Task.Delay(120);
                return MockAq(city);
            }

            try
            {
                var url = $"https://api.openaq.org/v3/latest?city={Uri.EscapeDataString(city)}&limit=1";
                if (!string.IsNullOrWhiteSpace(_apiKey))
                {
                    if (!_http.DefaultRequestHeaders.Contains("X-API-Key"))
                        _http.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
                }

                using var res = await _http.GetAsync(url);
                res.EnsureSuccessStatusCode();
                await using var stream = await res.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                if (!root.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                    return MockAq(city);

                var first = results[0];

                string mainPollutant = "";
                double value = double.NaN;
                if (first.TryGetProperty("measurements", out var measurements) && measurements.GetArrayLength() > 0)
                {
                    foreach (var m in measurements.EnumerateArray())
                    {
                        var param = m.GetProperty("parameter").GetString() ?? "";
                        if (param.Equals("pm25", StringComparison.OrdinalIgnoreCase) ||
                            param.Equals("pm10", StringComparison.OrdinalIgnoreCase))
                        {
                            mainPollutant = param;
                            value = m.GetProperty("value").GetDouble();
                            break;
                        }
                    }

                    if (double.IsNaN(value))
                    {
                        var m0 = measurements[0];
                        mainPollutant = m0.GetProperty("parameter").GetString() ?? "";
                        value = m0.GetProperty("value").GetDouble();
                    }
                }

                int aqiEstimate = double.IsNaN(value) ? -1 : (int)Math.Round(value);

                var model = new AQModel
                {
                    City = city,
                    AQI = aqiEstimate,
                    Category = MapCategory(aqiEstimate),
                    MainPollutant = mainPollutant,
                    FetchedAt = DateTime.UtcNow,
                    HourlyValues = new List<int> { aqiEstimate }
                };

                return model;
            }
            catch
            {
                return MockAq(city);
            }
        }

        private static string MapCategory(int aqi) =>
            aqi switch
            {
                <= 0 => "Unknown",
                <= 50 => "Good",
                <= 100 => "Moderate",
                <= 150 => "Unhealthy for Sensitive Groups",
                <= 200 => "Unhealthy",
                <= 300 => "Very Unhealthy",
                _ => "Hazardous"
            };

        private static AQModel MockAq(string city)
        {
            var rnd = new Random();
            var aqi = rnd.Next(20, 120);
            return new AQModel
            {
                City = city,
                AQI = aqi,
                Category = aqi < 50 ? "Good" : "Moderate",
                MainPollutant = "PM2.5",
                FetchedAt = DateTime.UtcNow,
                HourlyValues = new List<int> { aqi, aqi - 2, aqi + 3 }
            };
        }
    }
}