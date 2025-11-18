using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SmartCity.Services;
using SmartCity.ViewModels;
using SmartCity.Converters;

namespace SmartCity
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var weatherKey = "8503c207d70e09f2d6031c7649e4752d";
            var aqKey = "1b8fb03e10ce44422b0425f00dbd02500d71197e1f445d22878b06aa1b822e7c";
            var newsKey = "fb012f2a2eaa4cc6a5a6b2ee9471ba7f";
            var googleKey = "AIzaSyBqiSFZXWKxURzmbE9wN1X3e6szTIBlMzw";

            builder.Services.AddSingleton<IWeatherService>(_ => new WeatherService(weatherKey));
            builder.Services.AddSingleton<IAQService>(_ => new AQService(aqKey));
            builder.Services.AddSingleton<INewsService>(_ => new NewsService(newsKey));
            builder.Services.AddSingleton<IParkingService>(_ => new ParkingService(googleKey));

            builder.Services.AddSingleton<CityStateManager>();

            builder.Services.AddSingleton<WeatherViewModel>();
            builder.Services.AddSingleton<AQViewModel>();
            builder.Services.AddSingleton<NewsViewModel>();
            builder.Services.AddSingleton<ParkingViewModel>();

            builder.Services.AddSingleton<HasValueConverter>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}