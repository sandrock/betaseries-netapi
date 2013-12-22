using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Srk.BetaseriesApiApp.Converters {
    public class BoolToColorConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value != null &&  value is bool) {
                return (bool)value ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
