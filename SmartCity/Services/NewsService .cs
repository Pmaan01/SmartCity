using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SmartCity.Models;

namespace SmartCity.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _http = new();
        private readonly string? _apiKey;

        public NewsService() => _apiKey = null;

        public NewsService(string? apiKey) => _apiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey;

        public async Task<IEnumerable<NewsArticle>> GetLatestAsync(string keyword = "news")
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                return MockArticles();

            try
            {
                var q = Uri.EscapeDataString(string.IsNullOrWhiteSpace(keyword) ? "news" : keyword);
                var url = $"https://newsapi.org/v2/everything?q={q}&sortBy=publishedAt&pageSize=20&language=en&apiKey={_apiKey}";

                if (!_http.DefaultRequestHeaders.UserAgent.TryParseAdd("SmartCityApp/1.0 (mailto:maanparveen47@gmail.com)"))
                {
                    _http.DefaultRequestHeaders.UserAgent.ParseAdd("SmartCityApp/1.0");
                }
                _http.DefaultRequestHeaders.Accept.ParseAdd("application/json");

                System.Diagnostics.Debug.WriteLine("NewsService: Request URL = " + url);

                using var res = await _http.GetAsync(url);
                var content = await res.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"NewsService: HTTP {(int)res.StatusCode} {res.ReasonPhrase}");
                System.Diagnostics.Debug.WriteLine("NewsService: Response body (first 1000 chars): " + (content?.Length > 1000 ? content.Substring(0, 1000) : content));

                if (!res.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("NewsService: Non-success status -> returning mock articles.");
                    return MockArticles();
                }

                await using var stream = await res.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                var list = new List<NewsArticle>();

                if (root.TryGetProperty("articles", out var articles))
                {
                    foreach (var a in articles.EnumerateArray())
                    {
                        list.Add(new NewsArticle
                        {
                            Title = a.TryGetProperty("title", out var t) ? t.GetString() : "",
                            Source = a.TryGetProperty("source", out var s) && s.TryGetProperty("name", out var n) ? n.GetString() : "",
                            Url = a.TryGetProperty("url", out var u) ? u.GetString() : "",
                            ImageUrl = a.TryGetProperty("urlToImage", out var img) ? img.GetString() : "",
                            PublishedAt = a.TryGetProperty("publishedAt", out var p) && DateTime.TryParse(p.GetString(), out var dt)
                                ? dt
                                : DateTime.UtcNow
                        });
                    }
                }

                return list.Count > 0 ? list : MockArticles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("NewsService: Exception: " + ex);
                return MockArticles();
            }
        }


        private static IEnumerable<NewsArticle> MockArticles()
        {
            return new List<NewsArticle>
            {
                new NewsArticle { Title = "City installs new bike lanes", Source = "SmartCity News", PublishedAt = DateTime.Now.AddHours(-3) },
                new NewsArticle { Title = "Local park opens EV chargers", Source = "Daily Urban", PublishedAt = DateTime.Now.AddHours(-6) }
            };
        }
    }
}
