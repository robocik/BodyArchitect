using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BodyArchitect.Client.UI.Converters
{
    public class RemindBeforeToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan?) value;
            if(timeSpan==null || timeSpan==TimeSpan.Zero)
            {
                return 0;
            }
            if(timeSpan.Value<=TimeSpan.FromMinutes(5))
            {
                return 1;
            }
            if (timeSpan.Value <= TimeSpan.FromMinutes(10))
            {
                return 2;
            }
            if (timeSpan.Value <= TimeSpan.FromMinutes(15))
            {
                return 3;
            }
            if (timeSpan.Value <= TimeSpan.FromMinutes(30))
            {
                return 4;
            }
            if (timeSpan.Value <= TimeSpan.FromHours(1))
            {
                return 5;
            }
            if (timeSpan.Value <= TimeSpan.FromHours(18))
            {
                return 6;
            }
            if (timeSpan.Value <= TimeSpan.FromDays(1))
            {
                return 7;
            }
            return 8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int) value;
            switch (index)
            {
                case 0:
                    return null;
                case 1:
                    return TimeSpan.FromMinutes(5);
                case 2:
                    return TimeSpan.FromMinutes(10);
                case 3:
                    return TimeSpan.FromMinutes(15);
                case 4:
                    return TimeSpan.FromMinutes(30);
                case 5:
                    return TimeSpan.FromHours(1);
                case 6:
                    return TimeSpan.FromHours(18);
                case 7:
                    return TimeSpan.FromDays(1);
                default:
                    return TimeSpan.FromDays(7);
            }
        }
    }
}
