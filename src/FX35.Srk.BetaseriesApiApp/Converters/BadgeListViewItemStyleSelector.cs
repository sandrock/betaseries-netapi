using System.Windows;
using System.Windows.Controls;
using Srk.BetaseriesApiApp.ViewModels;

namespace Srk.BetaseriesApiApp.Converters {
    public class BadgeListViewItemStyleSelector : StyleSelector {

        public override Style SelectStyle(object item, DependencyObject container) {
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(container);
            var context = container.GetValue(FrameworkElement.DataContextProperty) as BadgeVM;
            string styleKey = "ListViewItemStyleBase";

            if (context != null) {
                //if (!context.IsVerified) {
                //    styleKey = "ListViewItemStyle_Gray";
                //} else
                if (context.IsImplemented) {
                    if (context.TranslatedName != null) {
                        if (context.TranslatedDescription != null) {
                            styleKey = context.IsVerified ? "ListViewItemStyle_Green" : "ListViewItemStyle_Blue";
                        } else {
                            styleKey = "ListViewItemStyle_Yellow";
                        }
                    } else {
                        styleKey = "ListViewItemStyle_Orange";
                    }
                } else {
                    styleKey = context.IsVerified ? "ListViewItemStyle_Red" : "ListViewItemStyle_Gray";
                }
            }
            return (Style)(ic.FindResource(styleKey));
        }

    }
}
