using CommunityToolkit.Mvvm.Messaging.Messages;
using KaliteKontrol.Models;

namespace KaliteKontrol.Messages
{
    public class AdimListesiniPLCyeGonderMessage : ValueChangedMessage<List<PcToPlcAdimYazilacaklar>>
    {
        public AdimListesiniPLCyeGonderMessage(List<PcToPlcAdimYazilacaklar> value) : base(value)
        {
        }
    }
}
