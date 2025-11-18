using SmartCity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartCity.Services
{
    public interface IParkingService
    {
        Task<IEnumerable<ParkingSpot>> GetNearbyAsync();
    }
}
