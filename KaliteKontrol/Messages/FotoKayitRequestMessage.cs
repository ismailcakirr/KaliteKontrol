using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows.Media.Imaging;

namespace KaliteKontrol.Messages
{
    public class FotoKayitRequestMessage : RequestMessage<bool>
    {
        public string SeriNo { get; set; }
        public WriteableBitmap WriteableBitmap { get; set; }

        public FotoKayitRequestMessage(string seriNo, WriteableBitmap writeableBitmap)
        {
            SeriNo = seriNo;
            WriteableBitmap = writeableBitmap;
        }

    }
}
