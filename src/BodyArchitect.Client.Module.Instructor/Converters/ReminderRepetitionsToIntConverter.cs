using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Converters
{
    class ReminderRepetitionsToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var repetitions = (ReminderRepetitions) value;
            return (int) repetitions;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var repetitions = (int)value;
            return (ReminderRepetitions)repetitions;
        }
    }
}
