using SmartCity.Models;
using SmartCity.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System;

namespace SmartCity.ViewModels
{
    public class AQViewModel : BaseViewModel
    {
        private readonly IAQService _service;
        private readonly CityStateManager _cityState;

        private AQModel _aq = new();
        public AQModel AQ { get => _aq; set => Set(ref _aq, value); }

        public ICommand RefreshCommand { get; }

        public AQViewModel(IAQService service)
        {
            _service = service;
            _cityState = CityStateManager.Instance;

            RefreshCommand = new Command(async () => await Load(_cityState.SelectedCity));

            _cityState.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CityStateManager.SelectedCity))
                {
                    _ = Load(_cityState.SelectedCity);
                }
            };

            _ = Load(_cityState.SelectedCity);
        }

        public async Task Load(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return;
            IsBusy = true;
            try
            {
                AQ = await _service.GetAqForLocationAsync(city);
                await CacheService.SaveAsync("aq.json", AQ);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AQ load error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadFromCache()
        {
            AQ = await CacheService.LoadAsync<AQModel>("aq.json") ?? new();
        }
    }
}
