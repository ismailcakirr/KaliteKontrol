using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO.Ports;

namespace KaliteKontrol.Services
{
    public class SerialPortService : BackgroundService
    {
        private readonly SerialPort _serialPort;
        private readonly ILogger<SerialPortService> _logger;
        private readonly AppSettings _settings;
        private bool _connectionStatus = false;

        public SerialPortService(ILogger<SerialPortService> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
            _serialPort = new SerialPort(_settings.ComPort, 9600, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500,
                Handshake = Handshake.None
            };
            _serialPort.DataReceived += SerialPort_DataReceived;
        } 
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);
            string data = _serialPort.ReadExisting();
            _logger.LogInformation("Seri Port Data Geldi:{data}, Uzunluk:{length}", data, data.Length);
            if (data.Length > 2)
            {
                WeakReferenceMessenger.Default.Send(new BarcodeChangedMessage(data.Trim()));
            }
        } 
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    if (_serialPort.IsOpen is false)
                    {
                        try
                        {
                            _serialPort.Open();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("SerialPort Open Error:{err}", ex.Message);
                            _serialPort.Close();
                            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                        } 
                    }

                    if (_connectionStatus != _serialPort.IsOpen)
                    {
                        _connectionStatus = _serialPort.IsOpen;
                        WeakReferenceMessenger.Default.Send(new BarcodeStatusChangedMessage(_connectionStatus));
                    } 
                }
                catch (Exception ex)
                {
                    _logger.LogError("Genel Hata:{err}", ex.Message);
                } 
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _serialPort.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}
