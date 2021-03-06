﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BodyArchitect.Client.Common;

namespace BodyArchitect.Client.UI.Converters
{
    public class DateTimeToRelativeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            return dateTime.ToLocalTime().ToRelativeDate();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
