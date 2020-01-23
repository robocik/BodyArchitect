using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BodyArchitect.Controls.External;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls.UserControls
{
    public interface ICalendarViewDayInfo
    {
        DateItem AddDayInfo(MonthCalendar calendar, TrainingDayDTO day);

        void PrepareData();
    }
}
