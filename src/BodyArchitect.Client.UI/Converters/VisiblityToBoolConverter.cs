using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BodyArchitect.Client.UI.Converters
{
    public class VisiblityToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="System.Windows.Visibility.Visible"/> to true and <see cref="System.Windows.Visibility.Collapsed"/> to false.
        /// </summary>
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Visibility)
            {
                return (Visibility)value == Visibility.Visible;
            }
            return value;
        }

        /// <summary>
        /// Converts true to <see cref="System.Windows.Visibility.Visible"/> and false to <see cref="System.Windows.Visibility.Collapsed"/>.
        /// </summary>
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                bool boolValue = (bool)value;
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return value;
        }
    }
}
