using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Media.Imaging;

namespace KaliteKontrol.Services
{
    public class CameraService : BackgroundService
    {
        private readonly ILogger<CameraService> _logger;
        private readonly AppSettings _settings;

        private readonly VideoCapture capture;


        private readonly string kameraIp;
        private readonly string rtspUrl;

        //Gönderliecek Bilglier
        private WriteableBitmap? writeableBitmap;
        private bool pingDurum = false;
        private bool _connectionStatus = false;
        private long sonIslemSuresiMs = 0;

        public CameraService(ILogger<CameraService> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
            kameraIp = _settings.KameraIp;
            rtspUrl = _settings.RtspUrl;
            capture = new VideoCapture();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            WeakReferenceMessenger.Default.Register<CameraService, CameraFrameRequestMessage>(this, (r, m) =>
            {
                m.Reply(r.writeableBitmap);
            });

            WeakReferenceMessenger.Default.Register<CameraService, CameraStatusRequestMessage>(this, (r, m) =>
            {
                m.Reply(r._connectionStatus);
            });

            var sw = new Stopwatch();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(100, stoppingToken);
                    pingDurum = await PingServerAsync(kameraIp, stoppingToken);
                    if (pingDurum == true)
                    {
                        sw.Restart();
                        writeableBitmap = await Task.Run(async () => await KameraIslemleri(stoppingToken));
                        sw.Stop();
                        sonIslemSuresiMs = sw.ElapsedMilliseconds;
                    }
                    else
                    {
                        if (capture.IsOpened())
                        {
                            capture.Release();
                        }
                        writeableBitmap = null;
                        _logger.LogInformation("Connection Status:False");
                        await Task.Delay(5000, stoppingToken);
                    }

                    if (_connectionStatus != pingDurum)
                    {
                        _connectionStatus = pingDurum;
                        WeakReferenceMessenger.Default.Send(new CameraStatusChangedMessage(_connectionStatus));
                        _logger.LogInformation("PingDurum:{durum}, Son işlem süresi:{sure}", pingDurum, sonIslemSuresiMs);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Genel Hata:{hata}", ex.Message);
                }
            }
        }

        private static async Task<bool> PingServerAsync(string adress, CancellationToken stoppingToken)
        {
            using var ping = new Ping();
            try
            {
                PingReply PR = await Task.Run(() => ping.SendPingAsync(adress), stoppingToken);
                if (PR.Status != IPStatus.Success)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<WriteableBitmap?> KameraIslemleri(CancellationToken stoppingToken)
        {
            WriteableBitmap? result = null;
            var newOpened = false;
            try
            {
                if (!capture.IsOpened())
                {
                    capture.Open(rtspUrl);
                    newOpened = true;
                }
                if (capture.IsOpened())
                {
                    if (newOpened == false)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            capture.Grab();
                        }
                    }
                    using var frameMat = capture.RetrieveMat();
                    result = frameMat.ToWriteableBitmap();
                    result.Freeze();
                }
                else
                {
                    capture.Release();
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("KameraIslemleri:{hata}", ex.Message);
                capture.Release();
            }
            return result;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
