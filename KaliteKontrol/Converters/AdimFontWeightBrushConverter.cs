using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KaliteKontrol.Converters
{
    public class AdimFontWeightBrushConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int adimNo && values[1] is int guncelAdim)
            {
                if (adimNo < guncelAdim)
                    return FontWeights.SemiBold; // Tamamlananlar
                else if (adimNo == guncelAdim)
                    return FontWeights.Bold; // Aktif
                else
                    return FontWeights.Normal; // Bekleyen
            }

            return FontWeights.Normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
