using CommunityToolkit.Mvvm.ComponentModel;
using KaliteKontrol.ModelsDb;

namespace KaliteKontrol.Models
{
    public partial class AdimUI : ObservableObject
    {
        public ADIM Adim { get; }

        public AdimUI(ADIM adim)
        {
            Adim = adim;
        }

        [ObservableProperty]
        private string _arkaPlanRengi = "Transparent";

        [ObservableProperty]
        private string _yaziRengi = "Black";

        [ObservableProperty]
        private string _yaziKalinligi = "Normal";

        [ObservableProperty]
        private string _adimDurumKind = "";
    }
}
