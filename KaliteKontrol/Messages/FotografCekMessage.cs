using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KaliteKontrol.Messages
{
    public class FotografCekMessage : ValueChangedMessage<bool>
    {
        public FotografCekMessage(bool value) : base(value)
        {
        }
    }
}
