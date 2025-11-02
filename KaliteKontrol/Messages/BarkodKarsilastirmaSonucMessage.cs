using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KaliteKontrol.Messages
{
    public class BarkodKarsilastirmaSonucMessage : ValueChangedMessage<bool>
    {
        public BarkodKarsilastirmaSonucMessage(bool value) : base(value)
        {
        }
    }
}
