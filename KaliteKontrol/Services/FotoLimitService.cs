using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace KaliteKontrol.Services
{
    public class FotoLimitService : BackgroundService
    {
        private readonly ILogger<FotoLimitService> _logger;

        private const long GB_1 = 1024 * 1024 * 1024L;  // 1Gb


        private readonly string fotoPath;
        private readonly long fotoLimitGb;
        private readonly string istNo;

        public FotoLimitService(ILogger<FotoLimitService> logger, IOptions<AppSettings> options)
        {
            _logger = logger;

            fotoPath = options.Value.FotoPath;
            fotoLimitGb = options.Value.FotoLimitGB * GB_1;
            istNo = $"Masa{options.Value.IstNo}";

        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            WeakReferenceMessenger.Default.Register<FotoLimitService, FotoKayitRequestMessage>(this, (r, m) =>
            {
                var sonuc = false;
                try
                {
                    var yol = ResimKayitYolGetir(m.SeriNo, istNo);
                    CreateThumbnail(yol, m.WriteableBitmap.Clone());
                    sonuc = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("FotoKayitRequestMessage Hata:{hata}", ex.Message);
                }
                m.Reply(sonuc);
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //await EskiResimleriSil();
                    await Task.Run(() => EskiResimleriSil(), stoppingToken);
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Genel Hata:{hata}", ex.Message);
                }
            }
        }

        private void EskiResimleriSil()
        {
            int adet = 0;
            if (Directory.Exists(fotoPath))
            {
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    string[] files = Directory.GetFiles(fotoPath, "*.*", SearchOption.AllDirectories);
                    List<FileInfo> fileInfoList = new();
                    long totalSize = 0;
                    foreach (string file in files)
                    {
                        FileInfo fi = new(file);
                        totalSize += fi.Length;
                        fileInfoList.Add(fi);
                    }

                    fileInfoList = fileInfoList.OrderBy(a => a.CreationTime).ToList();
                    foreach (FileInfo fi in fileInfoList)
                    {
                        if (totalSize > fotoLimitGb)
                        {
                            totalSize -= fi.Length;
                            fi.Delete();
                            adet++;
                        }
                        else
                            break;
                    }
                    sw.Stop();
                    _logger.LogInformation("EskiResimleriSil Toplam dosya adet:{adet}, Sure:{sure} ms", fileInfoList.Count, sw.ElapsedMilliseconds);
                    if (adet > 0)
                    {
                        _logger.LogInformation("{adet} adet dosya silindi", adet);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("EskiResimleriSil Hata:{hata}", ex.Message);
                }
            }
        }


        private string ResimKayitYolGetir(string seriNo, string sonek)
        {
            DateTime datetime = DateTime.Now;
            string yourPath = fotoPath;

            string yil = datetime.Year.ToString();
            string ay = datetime.Month.ToString();
            string gun = datetime.Day.ToString();
            string saat = datetime.Hour.ToString();
            string dakika = datetime.Minute.ToString();
            string saniye = datetime.Second.ToString();


            if (ay.Length == 1)
                ay = "0" + ay;
            if (gun.Length == 1)
                gun = "0" + gun;
            if (saat.Length == 1)
                saat = "0" + saat;
            if (dakika.Length == 1)
                dakika = "0" + dakika;
            if (saniye.Length == 1)
                saniye = "0" + saniye;

            yourPath = yourPath + @"\" + yil;
            if ((!Directory.Exists(yourPath)))
                Directory.CreateDirectory(yourPath);

            yourPath = yourPath + @"\" + ay;
            if ((!Directory.Exists(yourPath)))
                Directory.CreateDirectory(yourPath);

            yourPath = yourPath + @"\" + gun;
            if ((!Directory.Exists(yourPath)))
                Directory.CreateDirectory(yourPath);

            string resiYolu = yourPath + @"\" + seriNo + "_" + sonek + "_" + yil + ay + gun + saat + dakika + saniye + ".jpg";


            return resiYolu;
        }

        private static void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using FileStream stream5 = new(filename, FileMode.Create);
                PngBitmapEncoder encoder5 = new();
                encoder5.Frames.Add(BitmapFrame.Create(image5));
                encoder5.Save(stream5);
            }
        }
    }
} 
