using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BodyArchitect.Client.UI.Controls.Calendar.Common
{
    public class ChangeAppointmentTimeEventArgs : RoutedEventArgs
    {
        private TimeSpan changeTime;

        public ChangeAppointmentTimeEventArgs(TimeSpan changeTime)
        {
            this.changeTime = changeTime;
        }

        public TimeSpan ChangeTime
        {
            get { return changeTime; }
        }
    }
}
