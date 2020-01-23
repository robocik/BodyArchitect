using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BodyArchitect.Client.Module.StrengthTraining.Converters
{
    [ValueConversion(typeof(Guid), typeof(string))]
    public class GuidToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid guid = (Guid) value;
            return guid.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string) value;
            return new Guid(str);
        }

        #endregion
    }

    
}
