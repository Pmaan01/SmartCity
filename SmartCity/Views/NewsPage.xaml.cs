using Microsoft.Extensions.DependencyInjection;
using SmartCity.ViewModels;

namespace SmartCity.Views
{
    public partial class NewsPage : ContentPage
    {
        private readonly NewsViewModel vm;

        public NewsPage()
        {
            InitializeComponent();
            vm = App.Current.Services.GetRequiredService<NewsViewModel>();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnReadMoreClicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button button && !string.IsNullOrWhiteSpace(button.CommandParameter as string))
                {
                    string url = button.CommandParameter as string;
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        url = "https://" + url;
                    }
                    await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not open article: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"NewsPage Error: {ex}");
            }
        }
    }
}