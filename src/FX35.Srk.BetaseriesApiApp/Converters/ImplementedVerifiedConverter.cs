
namespace Srk.BetaseriesApiApp.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;
    using Srk.BetaseriesApiApp.ViewModels;

    public class ImplementedVerifiedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is BadgeVM)
            {
                var vm = (BadgeVM)value;
                if (vm.IsImplemented)
                    return vm.IsVerified ? "Verified & impl." : "Implemented";
                else
                    return vm.IsVerified ? "Verified" : "Not implemented";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
