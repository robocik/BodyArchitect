using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Converters
{
    class ReminderToIconConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    ReminderType type = (ReminderType) value;
        //    if(type==ReminderType.EntryObject)
        //    {
        //        return new BitmapImage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/ReminderCalendar16.png", UriKind.Absolute));
        //    }
        //    else if(type==ReminderType.ScheduleEntry)
        //    {
        //        return new BitmapImage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/ScheduleEntries.png", UriKind.Absolute));
        //    }
        //    else if (type == ReminderType.Birthday)
        //    {
        //        return new BitmapImage(new Uri("pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Birthday.png", UriKind.Absolute));
        //    }
        //    else
        //    {
        //        return new BitmapImage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Images/Reminder16.png", UriKind.Absolute));
        //    }
        //}
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ReminderType type = (ReminderType)value;
            return GetReminderImage(type);
        }

        public static Uri GetReminderImage(ReminderType type)
        {
            if (type == ReminderType.EntryObject)
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Resources;component/Images/ReminderCalendar32.png", UriKind.Absolute);
            }
            else if (type == ReminderType.ScheduleEntry)
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Resources;component/Images/ReminderScheduleEntries32.png", UriKind.Absolute);
            }
            else if (type == ReminderType.Birthday)
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Birthday.png", UriKind.Absolute);
            }
            else
            {
                return new Uri("pack://application:,,,/BodyArchitect.Client.Resources;component/Images/Reminder32.png", UriKind.Absolute);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
