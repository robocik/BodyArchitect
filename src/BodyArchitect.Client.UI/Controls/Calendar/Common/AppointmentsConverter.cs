using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace BodyArchitect.Client.UI.Controls.Calendar.Common
{
    /// <summary>
    /// Gets the appointments for the specified date.
    /// </summary>
    [ValueConversion(typeof(ObservableCollection<DateInfo>), typeof(ObservableCollection<DateInfo>))]
    public class AppointmentsConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<DateInfo> appointments = new ObservableCollection<DateInfo>();
            if(values[1]==null || !(values[1] is DateTime))
            {
                return appointments;
            }
            DateTime date = (DateTime)values[1];

            
            foreach (var appointment in (ObservableCollection<DateInfo>)values[0])
            {
                if (appointment.Date.Date == date)
                {
                    appointments.Add(appointment);
                }
            }

            return appointments;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
