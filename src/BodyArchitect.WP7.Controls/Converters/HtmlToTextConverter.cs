using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
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
    public class HtmlToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string comment = (string)value;
            if (string.IsNullOrEmpty(comment))
            {
                return comment;
            }
            if (comment.Length > 100)
            {
                comment = comment.Substring(0, 100) + "...";
            }
            return Regex.Replace(comment, @"<[^>]*>", String.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
