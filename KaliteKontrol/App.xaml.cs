using KaliteKontrol.ModelsDb;
using KaliteKontrol.Services;
using KaliteKontrol.Settings;
using KaliteKontrol.Viewmodels;
using KaliteKontrol.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace KaliteKontrol
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost? _host;

        private readonly Mutex _mutex;
        private bool createdNew;

        public App()
        {
            var appName = $"{typeof(App).Namespace}";
            _mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Uygulamanın sadece bir tane örneği çalışabilir.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                //Environment.Exit(0);
                Current.Shutdown();
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                var configuration = builder.Build();

                _host = Host.CreateDefaultBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddConfiguration(configuration);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        ConfigureServices(services, configuration);
                    })
                    .Build();
            }
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            services.AddSingleton<MainWindow>();
            services.AddTransient<BarkodView>();

            var conMsSqlServer = configuration.GetConnectionString("conMsSql");

            services.AddDbContext<KaliteContext>(options =>
            {
                options.UseSqlServer(conMsSqlServer);
            }, ServiceLifetime.Transient);

            var conMsSqlServerLocal = configuration.GetConnectionString("conMsSql2");

            services.AddDbContext<LocalContext>(options =>
            {
                options.UseSqlServer(conMsSqlServerLocal);
            }, ServiceLifetime.Transient);

            services.AddTransient<MainViewModel>();
            services.AddTransient<BarkodViewModel>();

            services.AddHostedService<SerialPortService>();

        }
                                                    
        protected async override void OnStartup(StartupEventArgs e)
        {
            if (_host != null)
            {
                //await _host.MigrateDatabase();
                await _host.StartAsync();

                //db oluşturma
                DbInitializer.Initialize(_host.Services);

                var mainVidow = _host.Services.GetRequiredService<MainWindow>();
                var logger = _host.Services.GetRequiredService<ILogger<App>>();
                logger.LogInformation("Program açıldı.");
                mainVidow.Show();
            }
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }

}
