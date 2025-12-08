using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SmartCity.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

namespace SmartCity.Services
{
    public class ParkingService : IParkingService
    {
        private readonly HttpClient _http = new();
        private readonly string? _apiKey;

        public ParkingService() => _apiKey = null;

        public ParkingService(string? apiKey) => _apiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;

        public async Task<IEnumerable<ParkingSpot>> GetNearbyAsync()
        {
            System.Diagnostics.Debug.WriteLine("ParkingService: GetNearbyAsync called");

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                System.Diagnostics.Debug.WriteLine("ParkingService: No API key, returning mock spots");
                await Task.Delay(150);
                return MockSpots();
            }

            try
            {
                // Request location permission
                System.Diagnostics.Debug.WriteLine("ParkingService: Requesting location permission");
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                System.Diagnostics.Debug.WriteLine($"ParkingService: Initial permission status: {status}");

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    System.Diagnostics.Debug.WriteLine($"ParkingService: After request, permission status: {status}");
                }

                if (status != PermissionStatus.Granted)
                {
                    System.Diagnostics.Debug.WriteLine("ParkingService: Location permission denied, returning mock spots");
                    return MockSpots();
                }

                // Get location with proper timeout and error handling
                System.Diagnostics.Debug.WriteLine("ParkingService: Getting device location");
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location == null)
                {
                    System.Diagnostics.Debug.WriteLine("ParkingService: Location is null, returning mock spots");
                    return MockSpots();
                }

                var lat = location.Latitude;
                var lon = location.Longitude;
                System.Diagnostics.Debug.WriteLine($"ParkingService: Got location - Lat: {lat}, Lon: {lon}");

                // Call Google Places API
                var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={lat},{lon}&radius=2000&type=parking&key={_apiKey}";
                System.Diagnostics.Debug.WriteLine($"ParkingService: Calling API: {url}");

                using var res = await _http.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"ParkingService: API returned {res.StatusCode}");
                    return MockSpots();
                }

                await using var stream = await res.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                var list = new List<ParkingSpot>();

                if (root.TryGetProperty("results", out var results))
                {
                    System.Diagnostics.Debug.WriteLine($"ParkingService: Found {results.GetArrayLength()} parking spots");

                    foreach (var r in results.EnumerateArray())
                    {
                        var name = r.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                        var vicinity = r.TryGetProperty("vicinity", out var v) ? v.GetString() ?? "" : "";
                        double plat = 0, plon = 0;

                        if (r.TryGetProperty("geometry", out var geom) &&
                            geom.TryGetProperty("location", out var loc) &&
                            loc.TryGetProperty("lat", out var platEl) &&
                            loc.TryGetProperty("lng", out var plonEl))
                        {
                            plat = platEl.GetDouble();
                            plon = plonEl.GetDouble();
                        }

                        var spot = new ParkingSpot
                        {
                            Name = name,
                            Address = vicinity,
                            Lat = plat,
                            Lon = plon,
                            Hours = "7:00 AM - 11:00 PM",
                            Notes = "Check availability online"
                        };

                        list.Add(spot);
                    }
                }

                if (list.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("ParkingService: No results from API, returning mock spots");
                    return MockSpots();
                }

                System.Diagnostics.Debug.WriteLine($"ParkingService: Returning {list.Count} real parking spots");
                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ParkingService: Exception - {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ParkingService: StackTrace: {ex.StackTrace}");
                return MockSpots();
            }
        }

        private static IEnumerable<ParkingSpot> MockSpots()
        {
            return new List<ParkingSpot>
            {
                new ParkingSpot
                {
                    Name = "Main Street Lot",
                    Address = "123 Main St, Surrey",
                    Hours = "24/7",
                    Notes = "Free after 6pm",
                    Lat = 49.2827,
                    Lon = -123.1207,
                    Price = 5.00m,
                    AvailableSpots = 45
                },
                new ParkingSpot
                {
                    Name = "City Hall Garage",
                    Address = "200 Civic Plaza, Surrey",
                    Hours = "7am–11pm",
                    Notes = "First hour free",
                    Lat = 49.2835,
                    Lon = -123.1161,
                    Price = 3.50m,
                    AvailableSpots = 12
                },
                new ParkingSpot
                {
                    Name = "Central Market Parking",
                    Address = "1500 Central Ave, Surrey",
                    Hours = "24/7",
                    Notes = "Flat rate $8 per day",
                    Lat = 49.2850,
                    Lon = -123.1180,
                    Price = 8.00m,
                    AvailableSpots = 78
                },
                new ParkingSpot
                {
                    Name = "Station Plaza",
                    Address = "800 Station Way, Surrey",
                    Hours = "6am–12am",
                    Notes = "Monthly passes available",
                    Lat = 49.2810,
                    Lon = -123.1220,
                    Price = 4.00m,
                    AvailableSpots = 23
                }
            };
        }
    }
}