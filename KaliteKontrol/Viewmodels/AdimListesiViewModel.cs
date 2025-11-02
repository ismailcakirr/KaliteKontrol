using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KaliteKontrol.Messages;
using KaliteKontrol.Models;
using KaliteKontrol.ModelsDb;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KaliteKontrol.Viewmodels
{
    public partial class AdimListesiViewModel : ObservableObject, IPageControl
    {
        private readonly ILogger<AdimListesiViewModel> _logger;
        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private ObservableCollection<AdimUI> _adimList = new();

        [ObservableProperty]
        public string _pageName = "ADIM LİSTESİ";

        [ObservableProperty]
        private List<ADIM>? _adimlar;

        [ObservableProperty]
        private string _sonAltBarkod = string.Empty;
        private bool barkodOkundu = false;

        [ObservableProperty]
        private BitmapImage? _aktifResim;

        [ObservableProperty]
        private PlcToPcAnlik? _plctoPcAnlik = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AktifAdim))] // AktifAdim da güncellensin
        private int _aktifAdimNo;

        public AdimUI? AktifAdim => AdimList.FirstOrDefault(a => a.Adim.ADIM_NO == AktifAdimNo);

        public AdimListesiViewModel(ILogger<AdimListesiViewModel> logger)
        {
            _logger = logger;

            WeakReferenceMessenger.Default.Register<AdimListesiViewModel, BarcodeChangedMessage>(this, (r, m) =>
            {
                r.SonAltBarkod = m.Value;
                r.barkodOkundu = true;
            });

            WeakReferenceMessenger.Default.Register<AdimListesiViewModel, PlcdenOkunacaklarMessage>(this, (r, m) =>
            {
                r.PlctoPcAnlik = m.Value;

                r.AktifAdimNo = m.Value.AktifAdimNo;
            });

            WeakReferenceMessenger.Default.Register<AdimListesiViewModel, AdimListesiGonderMessage>(this, (r, m) =>
            {
                r.Adimlar = m.Value;
                if (Adimlar is not null)
                {
                    LoadAdimList(Adimlar);
                }
            });

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += Timer_Tick;
        }

        private void LoadAdimList(List<ADIM> adimlar)
        {
            var adimlarDuzenli = adimlar.TakeWhile(a => a.ISLEM_NO != 0 && a.ISLEM_ADI != "Yok").Select(a => new AdimUI(a));
            AdimList = new ObservableCollection<AdimUI>(adimlarDuzenli);
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();
            if (Adimlar != null)
            {
                if (PlctoPcAnlik != null)
                {
                    switch (PlctoPcAnlik.AktifIslemNo)
                    {
                        case 1:

                            break;

                        case short n when (n >= 2 && n <= 50):
                            if (barkodOkundu == true)
                            {
                                barkodOkundu = false;
                                var aktifAdim = AdimList.FirstOrDefault(x => x.Adim.ADIM_NO == AktifAdimNo)?.Adim;

                                bool karsilastirmaSonucu = aktifAdim != null
                                    ? AltBarkodKarsilastir(aktifAdim.BC_TANIM, SonAltBarkod)
                                    : false;
                                WeakReferenceMessenger.Default.Send(new BarkodKarsilastirmaSonucMessage(karsilastirmaSonucu));
                            }
                            break;
                        case short n when (n >= 51 && n <= 100):
                            await Task.Delay(10);
                            break;
                        case short n when (n >= 101 && n <= 150):
                            break;
                        case short n when (n >= 151 && n <= 200):
                            break;
                        case short n when (n >= 201 && n <= 210):
                            break;
                        case short n when (n >= 300 && n <= 351):
                            break;
                        case 403:
                            break;
                        case 406:
                            break;


                        default:
                            break;
                    }
                }
            }

            _timer.Start();
        }

        private bool AltBarkodKarsilastir(string bcTanim, string sonAltBarkod)
        {
            bool sonuc = true;

            if (bcTanim.Length == sonAltBarkod.Length)
            {
                for (int i = 0; i < bcTanim.Length; i++)
                {
                    if (bcTanim[i] == 'x' || bcTanim[i] == 'X' || bcTanim[i] == '*')
                        continue;

                    if (bcTanim[i] != sonAltBarkod[i])
                    {
                        sonuc = false;
                        break;
                    }
                }
            }
            else
            {
                sonuc = false;
            }
            return sonuc;
        }
        [RelayCommand]
        public void AdimAtla()
        {
            AktifAdimNo += 1;
            AktifResim = AktifAdim != null
                && !string.IsNullOrWhiteSpace(AktifAdim.Adim.RESIM_YOLU)
                ? new BitmapImage(new Uri(AktifAdim.Adim.RESIM_YOLU, UriKind.Absolute)) : null;
            foreach (var item in AdimList)
            {
                if (item.Adim.ADIM_NO < AktifAdimNo)
                {
                    item.ArkaPlanRengi = "Lime";
                    item.YaziRengi = "White";
                    item.YaziKalinligi = "Bold";
                    item.AdimDurumKind = "CheckBold";
                }
                else if (item.Adim.ADIM_NO == AktifAdimNo)
                {
                    item.ArkaPlanRengi = "Yellow";
                    item.YaziRengi = "Red";
                    item.YaziKalinligi = "Bold";
                    item.AdimDurumKind = "TimerSandEmpty";
                }
                else
                {
                    item.ArkaPlanRengi = "Transparent";
                    item.YaziRengi = "Black";
                    item.YaziKalinligi = "Normal";
                    item.AdimDurumKind = "TimerSandEmpty";
                }
            }
        }

        [RelayCommand]
        public void SayfaDegis()
        {
            WeakReferenceMessenger.Default.Send(new NavigateToPageMessage(Constants.Kamera));
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
