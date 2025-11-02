using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KaliteKontrol.Messages
{
    public class CameraStatusChangedMessage : ValueChangedMessage<bool>
    {
        public CameraStatusChangedMessage(bool value) : base(value)
        {
        }
    }
}
