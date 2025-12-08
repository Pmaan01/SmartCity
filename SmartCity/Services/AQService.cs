using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string BaseUrl = "https://api.openaq.org/v3";

        private static readonly Dictionary<string, (double lat, double lon)> CityCoordinates = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Vancouver", (49.2827, -123.1207) },
            { "Toronto", (43.6532, -79.3832) },
            { "Montreal", (45.5017, -73.5673) },
            { "Calgary", (51.0447, -114.0719) },
            { "Edmonton", (53.5461, -113.4938) },
            { "London", (51.5074, -0.1278) },
            { "Paris", (48.8566, 2.3522) },
            { "Tokyo", (35.6895, 139.6917) },
            { "New York", (40.7128, -74.0060) },
            { "Los Angeles", (34.0522, -118.2437) },
            { "Delhi", (28.7041, 77.1025) },
            { "Beijing", (39.9042, 116.4074) }
        };

        public AQService() => _apiKey = null;
        public AQService(string? apiKey) => _apiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;

        public async Task<AQModel> GetAqForLocationAsync(string city)
        {
            System.Diagnostics.Debug.WriteLine($"AQService: API Key present: {!string.IsNullOrWhiteSpace(_apiKey)}");

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                System.Diagnostics.Debug.WriteLine($"AQService: No API key, returning mock data for {city}");
                await Task.Delay(120);
                return MockAq(city);
            }

            try
            {
                if (!_http.DefaultRequestHeaders.Contains("X-API-Key"))
                    _http.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

                var locUrlByCity = $"{BaseUrl}/locations?city={Uri.EscapeDataString(city)}&limit=1";
                System.Diagnostics.Debug.WriteLine($"AQService: Trying locations by city: {locUrlByCity}");
                using var locResp = await _http.GetAsync(locUrlByCity);
                if (locResp.IsSuccessStatusCode)
                {
                    using var stream = await locResp.Content.ReadAsStreamAsync();
                    using var doc = await JsonDocument.ParseAsync(stream);
                    if (doc.RootElement.TryGetProperty("results", out var res) && res.GetArrayLength() > 0)
                    {
                        var locationId = res[0].GetProperty("id").GetInt32();
                        System.Diagnostics.Debug.WriteLine($"AQService: Found location id {locationId} for city {city}");
                        var model = await FetchMeasurementsForLocationId(locationId, city);
                        if (model != null) return model;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"AQService: No locations found via city for {city}");
                    }
                }
                else
                {
                    var status = (int)locResp.StatusCode;
                    System.Diagnostics.Debug.WriteLine($"AQService: locations by city failed: {status}");
                }

                if (CityCoordinates.TryGetValue(city, out var coords))
                {
                    var locUrlByCoords = $"{BaseUrl}/locations?coordinates={coords.lat},{coords.lon}&radius=20000&limit=1";
                    System.Diagnostics.Debug.WriteLine($"AQService: Trying locations by coords: {locUrlByCoords}");
                    using var locResp2 = await _http.GetAsync(locUrlByCoords);
                    if (locResp2.IsSuccessStatusCode)
                    {
                        using var stream = await locResp2.Content.ReadAsStreamAsync();
                        using var doc = await JsonDocument.ParseAsync(stream);
                        if (doc.RootElement.TryGetProperty("results", out var res2) && res2.GetArrayLength() > 0)
                        {
                            var locationId = res2[0].GetProperty("id").GetInt32();
                            System.Diagnostics.Debug.WriteLine($"AQService: Found location id {locationId} by coords for {city}");
                            var model = await FetchMeasurementsForLocationId(locationId, city);
                            if (model != null) return model;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"AQService: locations by coords failed for {city}: {(int)locResp2.StatusCode}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"AQService: No coordinate mapping for {city}");
                }

                var measUrl = $"{BaseUrl}/measurements?city={Uri.EscapeDataString(city)}&limit=10&sort=desc";
                System.Diagnostics.Debug.WriteLine($"AQService: Trying measurements by city: {measUrl}");
                using var measResp = await _http.GetAsync(measUrl);
                if (measResp.IsSuccessStatusCode)
                {
                    using var stream = await measResp.Content.ReadAsStreamAsync();
                    using var doc = await JsonDocument.ParseAsync(stream);
                    if (doc.RootElement.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
                    {
                        return ParseMeasurements(results.EnumerateArray(), city);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"AQService: No measurements found for city {city}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"AQService: measurements by city failed: {(int)measResp.StatusCode}");
                }

                System.Diagnostics.Debug.WriteLine($"AQService: Falling back to mock for {city}");
                return MockAq(city);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AQService: Exception - {ex.Message}");
                return MockAq(city);
            }
        }

        private async Task<AQModel?> FetchMeasurementsForLocationId(int locationId, string city)
        {
            try
            {
                var url = $"{BaseUrl}/locations/{locationId}/latest";
                System.Diagnostics.Debug.WriteLine($"AQService: Fetching latest for location {locationId}: {url}");
                using var resp = await _http.GetAsync(url);
                var status = (int)resp.StatusCode;

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"AQService: locations/{{id}}/latest failed: {status} - {body}");
                    return null;
                }

                using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                if (!doc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                {
                    System.Diagnostics.Debug.WriteLine("AQService: latest results empty or missing");
                    return null;
                }

                var hourlyValues = new List<int>();
                foreach (var reading in results.EnumerateArray())
                {
                    if (reading.TryGetProperty("value", out var valueProp) && valueProp.ValueKind != JsonValueKind.Null)
                    {
                        var val = valueProp.GetDouble();
                        var aqiValue = (int)Math.Round(val);
                        hourlyValues.Add(aqiValue);
                        System.Diagnostics.Debug.WriteLine($"AQService: Parsed sensor value: {aqiValue}");
                    }
                }

                if (hourlyValues.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("AQService: No usable sensor values found");
                    return null;
                }

                var avgAqi = (int)Math.Round(hourlyValues.Average());
                System.Diagnostics.Debug.WriteLine($"AQService: Final AQI (average of {hourlyValues.Count} sensors): {avgAqi}");

                return new AQModel
                {
                    City = city,
                    AQI = avgAqi,
                    Category = MapCategory(avgAqi),
                    MainPollutant = "Mixed Sensors",
                    FetchedAt = DateTime.UtcNow,
                    HourlyValues = hourlyValues
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AQService: FetchMeasurementsForLocationId exception: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        private AQModel ParseMeasurements(JsonElement.ArrayEnumerator results, string city)
        {
            string mainPollutant = "";
            double? chosenValue = null;

            foreach (var item in results)
            {
                if (!item.TryGetProperty("parameter", out var paramProp) || !item.TryGetProperty("value", out var valueProp))
                    continue;

                var parameter = paramProp.GetString() ?? "";
                var value = valueProp.GetDouble();

                System.Diagnostics.Debug.WriteLine($"AQService: measurement found {parameter}: {value}");

                if (parameter.Equals("pm25", StringComparison.OrdinalIgnoreCase) ||
                    parameter.Equals("pm10", StringComparison.OrdinalIgnoreCase))
                {
                    mainPollutant = parameter;
                    chosenValue = value;
                    break;
                }

                if (chosenValue == null)
                {
                    mainPollutant = parameter;
                    chosenValue = value;
                }
            }

            if (chosenValue == null)
            {
                System.Diagnostics.Debug.WriteLine("AQService: no usable measurement values found, falling back to mock");
                return MockAq(city);
            }

            var aqiEstimate = (int)Math.Round(chosenValue.Value);
            System.Diagnostics.Debug.WriteLine($"AQService: Final AQI estimate = {aqiEstimate}");

            return new AQModel
            {
                City = city,
                AQI = aqiEstimate,
                Category = MapCategory(aqiEstimate),
                MainPollutant = mainPollutant,
                FetchedAt = DateTime.UtcNow,
                HourlyValues = new List<int> { aqiEstimate }
            };
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
            var mockData = new Dictionary<string, (int aqi, string pollutant)>(StringComparer.OrdinalIgnoreCase)
            {
                { "Vancouver", (45, "PM2.5") }, { "Toronto", (62, "PM2.5") }, { "Montreal", (58, "PM2.5") },
                { "Calgary", (52, "PM2.5") }, { "Edmonton", (55, "PM2.5") }, { "London", (68, "PM2.5") },
                { "Paris", (72, "PM2.5") }, { "Tokyo", (88, "PM2.5") }, { "New York", (75, "PM2.5") },
                { "Los Angeles", (95, "PM2.5") }, { "Sydney", (48, "PM2.5") }, { "Dubai", (120, "PM2.5") },
                { "Singapore", (82, "PM2.5") }, { "Bangkok", (105, "PM2.5") }, { "Delhi", (185, "PM2.5") },
                { "Mumbai", (142, "PM2.5") }, { "Beijing", (145, "PM2.5") }, { "Shanghai", (118, "PM2.5") },
                { "Mexico City", (95, "PM2.5") }, { "Berlin", (65, "PM2.5") }
            };

            var (aqi, pollutant) = mockData.TryGetValue(city, out var data) ? data : (75, "PM2.5");

            System.Diagnostics.Debug.WriteLine($"AQService: Returning MOCK data for {city}");

            return new AQModel
            {
                City = city,
                AQI = aqi,
                Category = MapCategory(aqi),
                MainPollutant = pollutant,
                FetchedAt = DateTime.UtcNow,
                HourlyValues = new List<int> { aqi, Math.Max(0, aqi - 5), aqi + 3 }
            };
        }
    }
}