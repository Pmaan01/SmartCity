using SmartCity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartCity.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsArticle>> GetLatestAsync(string query = "news");
    }
}
