using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.ModelsDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
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
        [NotifyPropertyChangedFor(nameof(ComDurum))]
        private bool _comStatus;
        public string ComDurum => ComStatus ? "Lime" : "Red";


        public MainViewModel(KaliteContext kaliteContext, ILogger<MainViewModel> logger, IServiceProvider serviceProvider)
        {
            _kaliteContext = kaliteContext;
            _logger = logger;
            _serviceProvider = serviceProvider;

            Pages.Add(_serviceProvider.GetRequiredService<BarkodViewModel>());

            CurrentPage = Pages[0];

            WeakReferenceMessenger.Default.Register<MainViewModel, BarcodeStatusChangedMessage>(this, (r, m) =>
            {
                
            }); 

            _timerAna = new DispatcherTimer();
            _timerAna.Interval = TimeSpan.FromMilliseconds(100);
            _timerAna.Tick += TimerAna_Tick;
            _timerAna.Start();

        }

        private void TimerAna_Tick(object? sender, EventArgs e)
        {
            _timerAna.Stop();

            _timerAna.Start();
        }
        
        [RelayCommand]
        public void BarkodOkut(string barcode)
        {
            WeakReferenceMessenger.Default.Send(new BarcodeChangedMessage(barcode));
        }
    }
}
