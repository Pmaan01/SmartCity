using SmartCity.Models;
using System.Threading.Tasks;

namespace SmartCity.Services
{
    public interface IAQService
    {
        Task<AQModel> GetAqForLocationAsync(string city);
    }
}
