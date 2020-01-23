using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BodyArchitect.Client.UI.Converters
{
    public class BoolToVisibilityMultiBinding : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool readOnly = false;
            foreach (object value in values)
            {
                if (value is bool)
                {
                    readOnly |= (bool)value;
                }
            }
            return readOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
