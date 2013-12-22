using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Srk.BetaseriesApiApp.Converters {
    public class PasswordParameterConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is PasswordBox) {
                return ((PasswordBox)value).Password;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
