using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.ModelsDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace KaliteKontrol.Viewmodels
{
    public partial class MainViewModel : ObservableRecipient
    {
        private readonly DispatcherTimer _timerAna;
        private readonly KaliteContext _kaliteContext;
        private readonly ILogger<MainViewModel> _logger;
        private readonly IServiceProvider _serviceProvider;


        [ObservableProperty]
        private ObservableCollection<Kullanicilar> _kullanicilar = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private Kullanicilar? _seciliKullanici;
        public bool LoginIzin => _seciliKullanici != null; //&& !HasErrors;



        [ObservableProperty]
        private Visibility _uyariGoster = Visibility.Hidden;


        [ObservableProperty]
        private ObservableCollection<IPageControl> _pages = [];

        private IPageControl? _currentPage;
        public IPageControl? CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != null)
                {
                    _currentPage.UnLoaded();
                }

                SetProperty(ref _currentPage, value);

                if (_currentPage != null)
                {
                    _currentPage.Loaded();
                }
            }
        }
        [ObservableProperty]
        private string _sonOkunanBarkod = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ComDurum))]
        private bool _comStatus;
        public string ComDurum => ComStatus ? "Lime" : "Red";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SqlDurum))]
        private bool _sqlStatus;
        public string SqlDurum => SqlStatus ? "Lime" : "Red";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PLCDurum))]
        private bool _plcStatus;
        public string PLCDurum => PlcStatus ? "Lime" : "Red";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(KameraDurum))]
        private bool _kameraStatus;
        public string KameraDurum => KameraStatus ? "Lime" : "Red";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowContent))]
        [NotifyPropertyChangedFor(nameof(ShowLogin))]
        [NotifyPropertyChangedFor(nameof(IsLogin))]
        private string? _username;

        public bool IsLogin => Username != null;

        public Visibility ShowLogin => IsLogin ? Visibility.Collapsed : Visibility.Visible;
        public Visibility ShowContent => IsLogin ? Visibility.Visible : Visibility.Collapsed;

        public MainViewModel(KaliteContext kaliteContext, ILogger<MainViewModel> logger, IServiceProvider serviceProvider)
        {
            _kaliteContext = kaliteContext;
            _logger = logger;
            _serviceProvider = serviceProvider;

            Pages.Add(_serviceProvider.GetRequiredService<BarkodViewModel>());
            Pages.Add(_serviceProvider.GetRequiredService<AdimListesiViewModel>());
            Pages.Add(_serviceProvider.GetRequiredService<KameraViewModel>());

            CurrentPage = Pages[0];

            WeakReferenceMessenger.Default.Register<MainViewModel, BarcodeStatusChangedMessage>(this, (r, m) =>
            {
                r.ComStatus = m.Value;
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, BarcodeChangedMessage>(this, (r, m) =>
            {
                r.SonOkunanBarkod = m.Value;
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, SqlStatusChangedMessage>(this, (r, m) =>
            {
                r.SqlStatus = m.Value;
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, CameraStatusChangedMessage>(this, (r, m) =>
            {
                r.KameraStatus = m.Value;
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, PlcStatusChangedMessage >(this, (r, m) =>
            {
                r.PlcStatus = m.Value;
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, NavigateToPageMessage>(this, (r, m) =>
            {
                r.CurrentPage = r.Pages[(int)m.Value - 1];
            });

            WeakReferenceMessenger.Default.Register<MainViewModel, UrunBilgiRequestMessage>(this, (r, m) =>
            {
                var seriNo = SonOkunanBarkod[10..];
                var urunKodu = SonOkunanBarkod[..10];   
                var urunBilgi = $"{seriNo}{urunKodu}";
                m.Reply(urunBilgi);
            });


            _timerAna = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timerAna.Tick += TimerAna_Tick;
            _timerAna.Start();

        }

        private void TimerAna_Tick(object? sender, EventArgs e)
        {
            _timerAna.Stop();

            _timerAna.Start();
        }

        [RelayCommand(CanExecute = nameof(LoginIzin))]
        private void Login(object parameter)
        {
            UyariGoster = Visibility.Hidden;
            if (parameter is PasswordBox passwordBox)
            {
                var password = passwordBox.Password ?? "";
                if (SeciliKullanici != null && SeciliKullanici.Sifre == password)
                {
                    Username = SeciliKullanici.KullaniciAdi;
                }
                else
                {
                    UyariGoster = Visibility.Visible;
                }
                passwordBox.Clear();
            }
        }

        [RelayCommand]
        public void BarkodOkut(string barcode)
        {
            WeakReferenceMessenger.Default.Send(new BarcodeChangedMessage(barcode));
        }

        [RelayCommand]
        private async Task Loaded()
        {
            //await Task.Delay(500);
            await KullanicilariGetirAsync();
            KameraStatus = WeakReferenceMessenger.Default.Send<CameraStatusRequestMessage>().Response;

            _timerAna.Start();
        }

        private async Task KullanicilariGetirAsync()
        {
            try
            {
                var kullanicilar = await _kaliteContext.Kullanicilar
                    .AsNoTracking()
                    //.Where(a => a.YetkiId == 3) //Sadece Kalite
                    .ToListAsync();
                Kullanicilar = new ObservableCollection<Kullanicilar>(kullanicilar);
                SeciliKullanici = Kullanicilar.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError("KullanicilariGetirAsync: {err}", ex.Message);
            }
            await Task.Delay(100);
        }

        //public void NavigateToPage(int index)
        //{
        //    if (index >= 0 && index < Pages.Count)
        //        CurrentPage = Pages[index];
        //}
    }
}
