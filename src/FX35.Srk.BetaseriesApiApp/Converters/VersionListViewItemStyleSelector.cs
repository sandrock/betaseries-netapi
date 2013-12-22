using System.Windows;
using System.Windows.Controls;
using Srk.BetaseriesApiApp.ViewModels;

namespace Srk.BetaseriesApiApp.Converters {
    public class VersionListViewItemStyleSelector : StyleSelector {

        public override Style SelectStyle(object item, DependencyObject container) {
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(container);
            var context = container.GetValue(FrameworkElement.DataContextProperty) as MethodVM;
            string styleKey = "ListViewItemStyleBase";

            if (context != null) {
                if (context.IsImplemented)
                    styleKey = context.IsUpToDate.Value ? 
                        "ListViewItemStyle_UpToDate" : 
                        "ListViewItemStyle_Outdated";
                else
                    styleKey = "ListViewItemStyle_NotImplemented";
            }
            return (Style)(ic.FindResource(styleKey));
        }

    }
}
