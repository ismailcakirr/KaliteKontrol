using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KaliteKontrol.Messages
{
    public class BarcodeStatusChangedMessage : ValueChangedMessage<bool>
    {
        public BarcodeStatusChangedMessage(bool value) : base(value)
        {
        }
    }

}
