using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.WP7.Controls.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hex = (string)value;

            var media = System.Windows.Media.Color.FromArgb(System.Convert.ToByte(hex.Substring(0, 2), 16), System.Convert.ToByte(hex.Substring(2, 2), 16), System.Convert.ToByte(hex.Substring(4, 2), 16), System.Convert.ToByte(hex.Substring(6, 2), 16));
            return new SolidColorBrush(media);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
