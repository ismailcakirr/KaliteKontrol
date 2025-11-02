using CommunityToolkit.Mvvm.Messaging.Messages;
using KaliteKontrol.Models;

namespace KaliteKontrol.Messages
{
    public class PlcdenOkunacaklarMessage : ValueChangedMessage<PlcToPcAnlik>
    {
        public PlcdenOkunacaklarMessage(PlcToPcAnlik value) : base(value)
        {
        }
    } 
}
