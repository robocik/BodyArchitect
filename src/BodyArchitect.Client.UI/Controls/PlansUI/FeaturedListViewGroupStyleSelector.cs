using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BodyArchitect.Client.UI.Controls.PlansUI
{
    public class FeaturedListViewGroupStyleSelector : StyleSelector
    {

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var normalStyle = (Style)Application.Current.Resources["listViewGroup"];
            var featuredStyle = (Style)Application.Current.Resources["featuredListViewGroup"];
            dynamic dynObj = item;
            IList<PlanViewModel> items = ((IEnumerable<object>)dynObj.Items).Cast<PlanViewModel>().ToList();
            if (items.Count > 0)
            {
                if (items[0].FeaturedType != FeaturedItem.None)
                {
                    return featuredStyle;
                }
            }
            return normalStyle;
        }

    }
}
