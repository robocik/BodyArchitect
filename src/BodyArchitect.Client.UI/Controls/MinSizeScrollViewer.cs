using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BodyArchitect.Client.UI.Controls
{
    public class MinSizeScrollViewer:ScrollViewer
    {
        public MinSizeScrollViewer()
        {
            SizeChanged += ScrollViewer_SizeChanged_1;
        }

        public static readonly DependencyProperty MinContentHeightProperty = DependencyProperty.Register("MinContentHeight", typeof(int),
            typeof(MinSizeScrollViewer), new UIPropertyMetadata() { PropertyChangedCallback = MinSizeChanged });
        public int MinContentHeight
        {
            get { return (int)GetValue(MinContentHeightProperty); }
            set { SetValue(MinContentHeightProperty, value); }
        }

        public static readonly DependencyProperty MinContentWidthProperty = DependencyProperty.Register("MinContentWidth", typeof(int),
            typeof(MinSizeScrollViewer), new UIPropertyMetadata() { PropertyChangedCallback = MinSizeChanged });
        public int MinContentWidth
        {
            get { return (int)GetValue(MinContentWidthProperty); }
            set { SetValue(MinContentWidthProperty, value); }
        }

        protected static void MinSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as MinSizeScrollViewer;
            if (null == self)
            {
                return;
            }
            self.Update();
        }

        private void ScrollViewer_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            if ((0 >= ActualHeight)
                || (0 >= ActualWidth))
            {
                // The attached ScrollViewer was probably not laid out yet, or has zero size.
                this.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                return;
            }

            int minHeight = MinContentHeight;
            int minWidth = MinContentWidth;

            if ( (minWidth <= 0))
            {
                // Probably our attached properties were not initialized. By default we disable the scrolling completely,
                // to prevent exceptions from infinitely-sized objects within us.
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                //return;
            }

            if ((minHeight <= 0) )
            {
                // Probably our attached properties were not initialized. By default we disable the scrolling completely,
                // to prevent exceptions from infinitely-sized objects within us.
                this.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                //return;
            }

            this.SizeChanged -= this.ScrollViewer_SizeChanged_1;

            if (this.ActualHeight < minHeight)
            {
                this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                MaxHeight = minHeight - (Margin.Bottom + Margin.Top);
            }
            else
            {
                this.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                MaxHeight = Double.PositiveInfinity;
            }

            if (this.ActualWidth < minWidth)
            {
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                MaxWidth = minWidth - (Margin.Left + Margin.Right);
            }
            else
            {
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                MaxWidth = Double.PositiveInfinity;
            }

            this.SizeChanged += this.ScrollViewer_SizeChanged_1;
        }
    }
}
