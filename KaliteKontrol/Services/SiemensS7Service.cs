using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.Models;
using KaliteKontrol.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using S7.Net;

namespace KaliteKontrol.Services
{
    public class SiemensS7Service : BackgroundService
    {
        private readonly ILogger<SiemensS7Service> _logger;
        private readonly AppSettings _settings;

        private Plc? plc;
        private string plcIpAdresi = string.Empty;
        private int dbNo = 0;
        private short rack = 0;
        private short slot = 0;
        private CpuType cpuType;
        private bool _connectionStatus;

        private bool AdimYazildiYazPos;
        private bool AdimYazildiYaz;

        private bool PyKaydedildiYazPos;
        private bool PyKaydedildiYaz;

        private bool BarkodKarsilastirmaSonucPos;
        private bool BarkodKarsilastirmaSonuc;


        private bool Button1Basildi = false;
        private bool Button1BasildiPos = false;

        public List<PcToPlcAdimYazilacaklar> pcToPlcAdimListesiList = new();
        public bool pcToPlcAdimListesiPos = false;


        public SiemensS7Service(ILogger<SiemensS7Service> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
            plcIpAdresi = _settings.PlcIp;
            dbNo = _settings.DbNo;
            cpuType = CpuType.S71500;
            plc = new Plc(cpuType, plcIpAdresi, rack, slot);


        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken: stoppingToken);

            
            WeakReferenceMessenger.Default.Register<SiemensS7Service, AdimListesiniPLCyeGonderMessage>(this, (r, m) =>
            {
                r.pcToPlcAdimListesiList = m.Value;
                pcToPlcAdimListesiPos = true;
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (plc != null && !plc.IsConnected)
                    {

                        _logger.LogInformation("IP:{ip}, Type:{type}, Rack:{rack}, Slot:{slot}", plc.IP, plc.CPU, rack, slot);
                        await plc.OpenAsync(stoppingToken);
                        _logger.LogInformation("PLC Bağlantısı Kuruldu.");


                        if (pcToPlcAdimListesiPos == true)
                        {
                            pcToPlcAdimListesiPos = false;

                            int offset = 0;
                            int classSize = 14;  

                            foreach (var adim in pcToPlcAdimListesiList)
                            {
                                await plc.WriteClassAsync(adim, dbNo, offset, stoppingToken);
                                offset += classSize;  
                            }

                            _logger.LogInformation("Adım listesi PLC'ye gönderildi.");
                        } 
                    } 
                }
                catch (Exception ex)
                {
                    _logger.LogError("Plc Connection Exception: {err}", ex.ToString());
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }

                try
                {
                    var status = plc != null && plc.IsConnected;
                    if (_connectionStatus != status)
                    {
                        _connectionStatus = status;
                        WeakReferenceMessenger.Default.Send(new PlcStatusChangedMessage(_connectionStatus));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Plc Message Exception: {err}", ex.ToString());

                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

