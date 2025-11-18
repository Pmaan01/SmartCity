using SmartCity.Models;
using SmartCity.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SmartCity.ViewModels
{
    public class ParkingViewModel : BaseViewModel
    {
        private readonly IParkingService _service;
        private readonly CityStateManager _cityState;

        public ObservableCollection<ParkingSpot> ParkingSpots { get; } = new();

        public ParkingViewModel(IParkingService service)
        {
            _service = service;
            _cityState = CityStateManager.Instance;

            _cityState.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CityStateManager.SelectedCity))
                {
                    _ = Load();
                }
            };

            _ = Load();
        }

        public async Task Load()
        {
            IsBusy = true;
            ParkingSpots.Clear();
            foreach (var spot in await _service.GetNearbyAsync())
                ParkingSpots.Add(spot);
            IsBusy = false;
        }
    }
}