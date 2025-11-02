using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KaliteKontrol.Viewmodels
{
    public partial class KameraViewModel : ObservableObject, IPageControl
    {
        private readonly DispatcherTimer _timer;
        private readonly ILogger<KameraViewModel> _logger;
        private readonly AppSettings _settings;

        [ObservableProperty]
        private bool _fotografCekildi = false;
        private bool fotografCekildiPos = false; 

        [ObservableProperty]
        public string _pageName = "KAMERA KONTROL";

        [ObservableProperty]
        private WriteableBitmap? _bitmap; 
        public KameraViewModel(ILogger<KameraViewModel> logger, IOptions<AppSettings> options)
        {
            _logger = logger;
            _settings = options.Value; 

            WeakReferenceMessenger.Default.Register<KameraViewModel, FotografCekMessage>(this, (r, m) =>
            {
                r.FotografCekildi = m.Value;
                fotografCekildiPos = true;
            });

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += Timer_Tick; 
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();

            if (fotografCekildiPos == true)
            {
                fotografCekildiPos = false;
                Bitmap = WeakReferenceMessenger.Default.Send<CameraFrameRequestMessage>().Response;
            }

            _timer.Start();
        } 

        [RelayCommand]
        private void ResimKayit()
        {
            var urunBilgi = WeakReferenceMessenger.Default.Send<UrunBilgiRequestMessage>().Response;
            if (Bitmap != null && urunBilgi != null  )
            { 

                WeakReferenceMessenger.Default.Send(new FotoKayitRequestMessage(urunBilgi, Bitmap));               
                Console.WriteLine("");
            }
        }

        [RelayCommand]
        public void SayfaDegis()
        {
            WeakReferenceMessenger.Default.Send(new NavigateToPageMessage(Constants.Barkod));
        }

        public void Loaded()
        {
            _timer.Start();
        }

        public void UnLoaded()
        {
            _timer.Stop();
        }
    }
}
