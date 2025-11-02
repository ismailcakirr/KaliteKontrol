using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KaliteKontrol.Converters
{
    public class AdimArkaBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int adimNo && values[1] is int guncelAdim)
            {
                if (adimNo < guncelAdim)
                    return Brushes.Lime; // Tamamlandı
                else if (adimNo == guncelAdim)
                    return Brushes.Yellow; // Aktif
                else
                    return Brushes.Transparent; // Bekleyen
            }
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
