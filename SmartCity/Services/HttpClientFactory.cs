using System;
using System.Net.Http;
using System.Text.Json;

namespace SmartCity.Services
{
    public static class HttpClientFactory
    {
        public static HttpClient Create() => new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
