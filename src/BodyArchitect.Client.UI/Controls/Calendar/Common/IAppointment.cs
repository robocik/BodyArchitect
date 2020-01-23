using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BodyArchitect.Client.UI.Controls.Calendar.Common
{
    public interface  IAppointment
    {
        DateTime StartTime { get; }

        DateTime EndTime { get; }

        bool ReadOnly { get; }
    }
  
}
