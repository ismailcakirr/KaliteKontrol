using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.Models;
using KaliteKontrol.ModelsDb;
using KaliteKontrol.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Windows;
using System.Windows.Threading;

namespace KaliteKontrol.Viewmodels
{
    public partial class BarkodViewModel : ObservableObject, IPageControl
    {
        private readonly DispatcherTimer _timerAna;
        private readonly DispatcherTimer _timerUyari;
        private readonly KaliteContext _kaliteContext;
        private readonly ILogger<BarkodViewModel> _logger;
        private readonly AppSettings _settings;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Mesaj))]
        private string? _hataMesaji;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ProgressVisibility))]
        [NotifyPropertyChangedFor(nameof(MessageVisibility))]
        private bool _dbProgress;

        private string borderClor = "Orange";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BorderColor))]
        private bool _uyariVar;

        [ObservableProperty]
        private string _pageName = "BARKOD KONTROL";

        [ObservableProperty]
        private string _sonOkunanBarkod = string.Empty;

        private string mesaj = "Ürün Barkodunu Okutunuz";

        public string BorderColor => UyariVar ? borderClor : "Transparent";
        public string Mesaj => HataMesaji ?? mesaj;
        public Visibility ProgressVisibility => DbProgress ? Visibility.Visible : Visibility.Collapsed;
        public Visibility MessageVisibility => DbProgress ? Visibility.Collapsed : Visibility.Visible;


        public string seriNo = string.Empty;
        public string urunKodu = string.Empty;
        private bool barkodOkundu = false;

        private List<ADIM> adimlar = new();
        public List<PcToPlcAdimYazilacaklar> pcToPlcAdimYazilacaklarList = new();
      
        public BarkodViewModel(KaliteContext kaliteContext, ILogger<BarkodViewModel> logger, IOptions<AppSettings> options)
        {
            _kaliteContext = kaliteContext;
            _logger = logger;
            _settings = options.Value;

            WeakReferenceMessenger.Default.Register<BarkodViewModel, BarcodeChangedMessage>(this, (r, m) =>
            {
                r.SonOkunanBarkod = m.Value;
                barkodOkundu = true;
            });

            _timerUyari = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timerUyari.Tick += UyariTick;
            _timerUyari.Start();

            _timerAna = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timerAna.Tick += TimerAna_Tick;

        }

        private void UyariTick(object? sender, EventArgs e)
        {
            UyariVar = !UyariVar;
        }

        private async void TimerAna_Tick(object? sender, EventArgs e)
        {
            _timerAna.Stop();

            if (barkodOkundu is true)
            {
                barkodOkundu = false;
                if (SonOkunanBarkod is not null && SonOkunanBarkod.Length >= 10)
                {
                    seriNo = SonOkunanBarkod[..10];
                    urunKodu = SonOkunanBarkod[10..];
                    //mesaj = $"Ürün Kodu: {urunKodu} Seri No: {seriNo}";
                    HataMesaji = null;

                    //adimlar = _kaliteContext.ADIM.Where(a => a.TIP_KODU == urunKodu).ToList();
                    adimlar = [.. _kaliteContext.ADIM.Where(a => a.TIP_KODU == urunKodu && a.IST_NO == _settings.IstNo)];
                    if (adimlar.Count > 0)
                    {
                        pcToPlcAdimYazilacaklarList.Clear();
                        foreach (var item in adimlar)
                        {
                            var yeniAdim = new PcToPlcAdimYazilacaklar
                            {
                                AktifIslemNo = (short)item.ISLEM_NO,
                                SikiciNo = (short)item.SIKICI_NO,
                                ProgNo = (short)item.PROG_NO,
                                Sure = (short)item.SURE,
                                ReworkSikiciNo = (short)item.REWORK_SIKICI_NO,
                                ReworkProgNo = (short)item.REWORK_PROG_NO
                            };
                            pcToPlcAdimYazilacaklarList.Add(yeniAdim);
                        }
                        WeakReferenceMessenger.Default.Send(new AdimListesiniPLCyeGonderMessage(pcToPlcAdimYazilacaklarList));
                        WeakReferenceMessenger.Default.Send(new AdimListesiGonderMessage(adimlar));
                        WeakReferenceMessenger.Default.Send(new NavigateToPageMessage(Constants.Adimlar));
                    }
                    else
                    {
                        borderClor = "Red";
                        HataMesaji = "Barkod Veritabanında Bulunamadı Tekrar Okutunuz";
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        SonOkunanBarkod = string.Empty;
                        HataMesaji = null;
                        mesaj = "Ürün Barkodunu Okutunuz";
                        borderClor = "Orange";
                    }
                }
                else
                {
                    borderClor = "Red";
                    HataMesaji = "Geçersiz Barkod Tekrar Okutunuz";
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    HataMesaji = null;
                    borderClor = "Orange";
                }
            }

            _timerAna.Start();
        }

        public void Loaded()
        {
            _timerAna.Start();
        }

        public void UnLoaded()
        {
            _timerAna.Stop();
        }

    }
}
