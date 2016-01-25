
namespace Srk.BetaseriesApiApp.Converters
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class PasswordParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is PasswordBox)
            {
                return ((PasswordBox)value).Password;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
