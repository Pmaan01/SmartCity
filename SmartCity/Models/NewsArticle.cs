using System;

namespace SmartCity.Models
{
    public class NewsArticle
    {
        public string Title { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
