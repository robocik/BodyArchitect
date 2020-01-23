using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Visiblox.Charts;

namespace BodyArchitect.Client.UI.Controls
{
    public class BAChart : Chart
    {
        public BAChart()
        {
            Style = (Style) Application.Current.Resources["BAChartStyle"];
            LegendVisibility = Visibility.Visible;
        }
    }
}
