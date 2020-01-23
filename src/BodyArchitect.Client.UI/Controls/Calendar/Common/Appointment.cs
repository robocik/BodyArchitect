using System;
using BodyArchitect.Client.Common;

namespace BodyArchitect.Client.UI.Controls.Calendar.Common
{

    public class DateInfo:ViewModelBase
    {
        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                NotifyOfPropertyChange(()=>ToolTip);
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
