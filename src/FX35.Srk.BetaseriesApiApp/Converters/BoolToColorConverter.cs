
namespace Srk.BetaseriesApiApp.Converters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                return (bool)value ? Brushes.Green : Brushes.Red;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
