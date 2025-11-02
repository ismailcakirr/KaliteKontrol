using CommunityToolkit.Mvvm.Messaging.Messages;
using KaliteKontrol.Viewmodels;

namespace KaliteKontrol.Messages
{
    public class NavigateToPageMessage : ValueChangedMessage<Constants>
    {
        public NavigateToPageMessage(Constants value) : base(value)
        {
        }
    }
}
