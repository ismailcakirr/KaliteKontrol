using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.ModelsDb;
using Microsoft.Extensions.Logging;
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
        public BarkodViewModel(KaliteContext kaliteContext, ILogger<BarkodViewModel> logger)
        {
            _kaliteContext = kaliteContext;
            _logger = logger;

            WeakReferenceMessenger.Default.Register<BarkodViewModel, BarcodeChangedMessage>(this, (r, m) =>
            {
                r.SonOkunanBarkod = m.Value;
                barkodOkundu = true;
            });

            _timerUyari = new DispatcherTimer();
            _timerUyari.Interval = TimeSpan.FromMilliseconds(500);
            _timerUyari.Tick += UyariTick;
            _timerUyari.Start();

            _timerAna = new DispatcherTimer();
            _timerAna.Interval = TimeSpan.FromMilliseconds(100);
            _timerAna.Tick += TimerAna_Tick;

        }

        private void UyariTick(object? sender, EventArgs e)
        {
            UyariVar = !UyariVar;
        }

        private void TimerAna_Tick(object? sender, EventArgs e)
        {
            _timerAna.Stop();

            if(barkodOkundu is true)
            {
                barkodOkundu = false;
                if(SonOkunanBarkod is not null && SonOkunanBarkod.Length >= 10)
                {
                    seriNo = SonOkunanBarkod[..10];
                    urunKodu = SonOkunanBarkod[10..];
                    mesaj = $"Ürün Kodu: {urunKodu} Seri No: {seriNo}";
                    HataMesaji = null;
                }
                else
                {
                    HataMesaji = "Geçersiz Barkod Tekrar Okutunuz";
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
