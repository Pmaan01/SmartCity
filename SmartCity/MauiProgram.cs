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

            var weatherKey = "API_KEY";
            var aqKey = "API_KEY";
            var newsKey = "API_KEY";
            var googleKey = "API_KEY";

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
