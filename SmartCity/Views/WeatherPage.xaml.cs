using Microsoft.Extensions.DependencyInjection;
using SmartCity.ViewModels;

namespace SmartCity.Views
{
    public partial class WeatherPage : ContentPage
    {
        private readonly WeatherViewModel vm;

        public WeatherPage()
        {
            InitializeComponent();
            vm = App.Current.Services.GetRequiredService<WeatherViewModel>();
            BindingContext = vm;

            CityEntry.Completed += async (s, e) =>
            {
                var q = string.IsNullOrWhiteSpace(vm.CityInput) ? vm.SelectedCity : vm.CityInput;
                if (string.IsNullOrWhiteSpace(q)) return;
                try
                {
                    await vm.Load(q);
                    UpdateWeatherTheme();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WeatherPage search error: {ex}");
                }
            };

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Weather))
                {
                    MainThread.BeginInvokeOnMainThread(() => UpdateWeatherTheme());
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await vm.LoadFromCache();
                var initial = vm.SelectedCity ?? vm.CityInput;
                if (string.IsNullOrWhiteSpace(initial)) initial = "Vancouver";
                await vm.Load(initial);
                UpdateWeatherTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WeatherPage OnAppearing error: {ex}");
            }
        }

        private void UpdateWeatherTheme()
        {
            if (vm?.Weather?.Condition == null) return;
            string condition = vm.Weather.Condition.ToLower();
            Color bgColor = GetWeatherBackgroundColor(condition);
            BackgroundFrame.Background = new SolidColorBrush(bgColor);
        }

        private Color GetWeatherBackgroundColor(string condition)
        {
            if (condition.Contains("sunny") || condition.Contains("clear"))
                return Color.FromArgb("#87CEEB");
            if (condition.Contains("cloud"))
                return Color.FromArgb("#A9A9A9");
            if (condition.Contains("rain"))
                return Color.FromArgb("#4A5568");
            if (condition.Contains("snow"))
                return Color.FromArgb("#B0E0E6");
            if (condition.Contains("thunder") || condition.Contains("storm"))
                return Color.FromArgb("#2C3E50");
            if (condition.Contains("fog") || condition.Contains("mist"))
                return Color.FromArgb("#808080");
            if (condition.Contains("partly"))
                return Color.FromArgb("#ADD8E6");
            if (condition.Contains("drizzle"))
                return Color.FromArgb("#708090");
            if (condition.Contains("haze"))
                return Color.FromArgb("#D2B48C");
            return Color.FromArgb("#87CEEB");
        }
    }
}