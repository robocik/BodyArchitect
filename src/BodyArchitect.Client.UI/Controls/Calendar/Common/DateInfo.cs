using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Common;

namespace BodyArchitect.Client.UI.Controls.Calendar.Common
{
    /// <summary>
    /// The appointment data.
    /// </summary>
    public class Appointment : DateInfo
    {
        public Appointment()
        {
            Brush = new SolidColorBrush(Colors.BlueViolet);
        }

        public string Subject { get; set; }

        public Brush Brush { get; set; }
    }




    public class DateInfo : ViewModelBase
    {
        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                NotifyOfPropertyChange(() => ToolTip);
            }
        }

        public object Tag { get; set; }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyOfPropertyChange(() => Date);
            }
        }
    }
}
