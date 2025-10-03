using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KaliteKontrol.Messages
{
    public class BarcodeChangedMessage : ValueChangedMessage<string>
    {
        public BarcodeChangedMessage(string value) : base(value)
        {
        }
    }
}
