using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.Service.Client.WP7
{
    public class DateEventArgs : EventArgs
    {
        public DateEventArgs(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; private set; }
    }
}
