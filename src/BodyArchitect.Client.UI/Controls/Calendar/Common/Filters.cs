using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.UI.Controls.Calendar.Common
{
    public static class Filters
    {
        public static IEnumerable<IAppointment> ByDate(this IEnumerable<IAppointment> appointments, DateTime date)
        {
            var app = from a in appointments
                      where a.StartTime.Date == date.Date
                      select a;
            return app;
        }
    }
}
