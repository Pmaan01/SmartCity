using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;

namespace SmartCity
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            MainPage = new AppShell();
        }

        // Expose a static, strongly typed reference to this App
        public static new App Current => (App)Application.Current;

        // Give your pages access to the DI provider
        public IServiceProvider Services => _serviceProvider;
    }
}
