using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using BodyArchitect.Client.Common;

namespace BodyArchitect.Client.UI.Converters
{
    public class ColorsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hex = (string) value;

            var media=System.Windows.Media.Color.FromArgb(System.Convert.ToByte(hex.Substring(0, 2), 16), System.Convert.ToByte(hex.Substring(2, 2), 16), System.Convert.ToByte(hex.Substring(4, 2), 16), System.Convert.ToByte(hex.Substring(6, 2), 16));
            return media;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.Color yourColor = (Color)value; // This is your color to convert from
            string hexColor = yourColor.A.ToString("X2") + yourColor.R.ToString("X2") + yourColor.G.ToString("X2") + yourColor.B.ToString("X2");
            return hexColor;
        }
    }
}
