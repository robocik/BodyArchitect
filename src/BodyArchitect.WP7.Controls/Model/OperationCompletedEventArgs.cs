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

namespace BodyArchitect.WP7.Controls.Model
{
    public class OperationCompletedEventArgs:EventArgs
    {
        public OperationCompletedEventArgs(bool error)
        {
            Error = error;
        }
        public bool Error { get; private set; }
    }
}
