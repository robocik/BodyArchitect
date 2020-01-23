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

namespace BodyArchitect.WP7.Controls
{
    public class ListReorderedEventArgs:EventArgs
    {
        public int FromIndex { get; private set; }

        public int ToIndex { get; private set; }

        public object Object { get; private set; }

        public ListReorderedEventArgs(int fromIndex, int toIndex, object o)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
            Object = o;
        }
    }
}
