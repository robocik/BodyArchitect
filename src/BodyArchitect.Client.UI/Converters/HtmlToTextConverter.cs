using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace BodyArchitect.Client.UI.Converters
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
                comment = comment.Substring(0, 100)+"...";
            }
            return Regex.Replace(comment, @"<[^>]*>", String.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
