using SmartCity.Models;
using SmartCity.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SmartCity.ViewModels
{
    public class WeatherViewModel : BaseViewModel
    {
        private readonly IWeatherService _weatherService;
        private readonly CityStateManager _cityState;

        private WeatherModel _weather = new();
        public WeatherModel Weather { get => _weather; set => Set(ref _weather, value); }

        public bool HasWeather => Weather?.City != null;

        public ObservableCollection<string> Cities { get; } = new()
        {
            "Vancouver", "Toronto", "Montreal", "Calgary", "Edmonton",
            "London", "Paris", "Tokyo", "New York", "Los Angeles",
            "Sydney", "Dubai", "Singapore", "Bangkok", "Delhi",
            "Mumbai", "Beijing", "Shanghai", "Mexico City", "Berlin"
        };

        private string _selectedCity = "Vancouver";
        public string SelectedCity
        {
            get => _selectedCity;
            set
            {
                if (!Equals(_selectedCity, value))
                {
                    _selectedCity = value;
                    OnPropertyChanged();
                    _cityState.SelectedCity = value;
                    _ = LoadWeather(value);
                }
            }
        }

        private string _cityInput = "";
        public string CityInput
        {
            get => _cityInput;
            set => Set(ref _cityInput, value);
        }

        private ObservableCollection<string> _searchSuggestions = new();
        public ObservableCollection<string> SearchSuggestions
        {
            get => _searchSuggestions;
            set => Set(ref _searchSuggestions, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        public WeatherViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;
            _cityState = CityStateManager.Instance;

            SearchCommand = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(CityInput))
                {
                    SelectedCity = CityInput;
                    CityInput = "";
                }
            });

            RefreshCommand = new Command(async () => await LoadWeather(SelectedCity));

            _cityState.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CityStateManager.SelectedCity))
                {
                    _selectedCity = _cityState.SelectedCity;
                    OnPropertyChanged(nameof(SelectedCity));
                    _ = LoadWeather(_cityState.SelectedCity);
                }
            };

            _ = LoadWeather(SelectedCity);
        }

        public void UpdateSearchSuggestions(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                SearchSuggestions.Clear();
                return;
            }

            var suggestions = Cities
                .Where(c => c.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .Take(5)
                .ToList();

            SearchSuggestions.Clear();
            foreach (var s in suggestions)
                SearchSuggestions.Add(s);
        }

        public async Task Load(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return;
            SelectedCity = city;
            await LoadWeather(city);
        }

        public async Task LoadFromCache()
        {
            try
            {
                var cached = await CacheService.LoadAsync<WeatherModel>("weather.json");
                if (cached != null)
                {
                    Weather = cached;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load from cache error: {ex}");
            }
        }

        private async Task LoadWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return;
            IsBusy = true;
            try
            {
                Weather = await _weatherService.GetWeatherForCityAsync(city);
                await CacheService.SaveAsync("weather.json", Weather);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Weather load error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}