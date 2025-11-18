using Microsoft.Extensions.DependencyInjection;
using SmartCity.ViewModels;

namespace SmartCity.Views
{
    public partial class AQPage : ContentPage
    {
        private readonly AQViewModel vm;

        public AQPage()
        {
            InitializeComponent();
            vm = App.Current.Services.GetRequiredService<AQViewModel>();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}