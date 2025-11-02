using CommunityToolkit.Mvvm.Messaging.Messages;
using KaliteKontrol.ModelsDb;

namespace KaliteKontrol.Messages
{
    public class AdimListesiGonderMessage : ValueChangedMessage<List<ADIM>>
    {
        public AdimListesiGonderMessage(List<ADIM> value) : base(value)
        {
        }
    }
}
