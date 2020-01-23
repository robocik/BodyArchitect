using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.UI.Controls.Calendar.Common
{
    public interface ICalendarControl
    {
        void ScrollToHome();

        bool ShowOffPeekHours { get; set; }

        int PeekStartHour { get; }

        int PeekEndHour { get; }

        IAppointment SelectedAppointment
        {
            get;
            set;
        }

        bool ReadOnly { get; }
    }
}
