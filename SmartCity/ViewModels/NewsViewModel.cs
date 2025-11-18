using CommunityToolkit.Mvvm.ComponentModel;
using SmartCity.Models;
using SmartCity.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SmartCity.ViewModels
{
    public partial class NewsViewModel : ObservableObject
    {
        private readonly INewsService _newsService;
        private readonly CityStateManager _cityState;

        public ObservableCollection<NewsArticle> Articles { get; } = new();

        [ObservableProperty]
        private string selectedCity = "Vancouver";

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string statusMessage = "Loading news...";

        public NewsViewModel(INewsService newsService)
        {
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            _cityState = CityStateManager.Instance;

            SelectedCity = _cityState.SelectedCity;

            _cityState.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CityStateManager.SelectedCity))
                {
                    SelectedCity = _cityState.SelectedCity;
                    _ = LoadNewsAsync(_cityState.SelectedCity);
                }
            };

            _ = LoadNewsAsync(SelectedCity);
        }

        private async Task LoadNewsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || IsBusy) return;

            try
            {
                IsBusy = true;
                StatusMessage = $"Loading news for \"{query}\"...";
                Articles.Clear();

                var results = await _newsService.GetLatestAsync(query);

                if (results != null)
                {
                    foreach (var a in results)
                        Articles.Add(a);
                }

                if (Articles.Count == 0)
                {
                    StatusMessage = $"No articles found for \"{query}\"";
                }
                else
                {
                    StatusMessage = $"Loaded {Articles.Count} articles for \"{query}\"";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load news: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"NewsViewModel error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}