using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KaliteKontrol.Messages
{
    public class PlcStatusChangedMessage : ValueChangedMessage<bool>
    {
        public PlcStatusChangedMessage(bool value) : base(value)
        {
        }
    }
}
