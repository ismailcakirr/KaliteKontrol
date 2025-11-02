using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KaliteKontrol.Messages
{
    public class SqlStatusChangedMessage : ValueChangedMessage<bool>
    {
        public SqlStatusChangedMessage(bool value) : base(value)
        {
        }
    } 
}
