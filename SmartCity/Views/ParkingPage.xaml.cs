using Microsoft.Extensions.DependencyInjection;
using SmartCity.Models;
using SmartCity.ViewModels;

namespace SmartCity.Views
{
    public partial class ParkingPage : ContentPage
    {
        private readonly ParkingViewModel vm;

        public ParkingPage()
        {
            InitializeComponent();
            vm = App.Current.Services.GetRequiredService<ParkingViewModel>();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.Load();
        }

        private async void OnGetDirectionsClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && button.CommandParameter is ParkingSpot spot)
                {
                    if (spot.Lat != 0 && spot.Lon != 0)
                    {
                        string mapsUrl = $"https://www.google.com/maps/search/?api=1&query={spot.Lat},{spot.Lon}&query_place_id={Uri.EscapeDataString(spot.Name)}";
                        await Browser.Default.OpenAsync(mapsUrl, BrowserLaunchMode.SystemPreferred);
                    }
                    else
                    {
                        await DisplayAlert("Directions", $"Opening directions for {spot.Name}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not open directions: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"ParkingPage Error: {ex}");
            }
        }
    }
}