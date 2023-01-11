using CodeSwine_Solo_Public_Lobby.Services;
using CodeSwine_Solo_Public_Lobby.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CodeSwine_Solo_Public_Lobby
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<IConnectionService, ConnectionService>();
            services.AddTransient<ICurrentIpService, CurrentIpService>();
            services.AddTransient<IFirewallService, FirewallService>();
            services.AddTransient<IHotkeyService, HotkeyService>();
            services.AddTransient<IIpHelperService, IpHelperService>();
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ISettingsService, SettingsService>();

            services.AddSingleton<MainWindow>();
        }
    }
}
