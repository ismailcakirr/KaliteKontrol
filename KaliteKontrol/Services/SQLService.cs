using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.ModelsDb;
using KaliteKontrol.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KaliteKontrol.Services
{
    public class SQLService : BackgroundService
    {
        private readonly ILogger<SQLService> _logger;
        private readonly IServiceProvider _serviceProvider; 
        private bool _connectionStatus;

        public SQLService(ILogger<SQLService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
           _serviceProvider = serviceProvider; 
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool firstLoad = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if(firstLoad is true )
                    {
                        firstLoad = false;
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                        using var scope = _serviceProvider.CreateScope();
                        using var dbContext = scope.ServiceProvider.GetRequiredService<KaliteContext>();
                        bool status = await dbContext.Database.CanConnectAsync(stoppingToken);   
                        if (_connectionStatus != status)
                        {
                            _connectionStatus=status;
                            _logger.LogInformation("SQL Bağlantı Durumu Değişti: {Status}", status);
                            WeakReferenceMessenger.Default.Send(new SqlStatusChangedMessage(_connectionStatus));
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("SQL Service Exception: {err}", ex.ToString());
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
