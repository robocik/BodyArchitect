using System;
using Xceed.Wpf.Toolkit;

namespace BodyArchitect.Client.UI.Controls
{
    public class TimeSpanUpDown : DateTimeUpDown
    {
        public TimeSpanUpDown()
        {
            Format = DateTimeFormat.Custom;
            FormatString = "HH:mm:ss";
        }
    }
}
