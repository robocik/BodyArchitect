﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BodyArchitect.Client.UI.Controls.Calendar.Controls
{
    class DragAdorner : Adorner
    {
        protected FrameworkElement _child;
        protected UIElement _owner;
        protected double XCenter;
        protected double YCenter;

        public DragAdorner(UIElement owner) : base(owner) { }

        public DragAdorner(UIElement owner, FrameworkElement adornElement, bool useVisualBrush, double opacity)
            : base(owner)
        {
            _owner = owner;
            if (useVisualBrush)
            {
                VisualBrush _brush = new VisualBrush(adornElement);
                _brush.Opacity = opacity;
                Rectangle r = new Rectangle();
                r.RadiusX = 3;
                r.RadiusY = 3;

                //TODO: questioning DesiredSize vs. Actual 
                r.Width = adornElement.ActualWidth;
                r.Height = adornElement.ActualHeight;

                //XCenter = adornElement.ActualWidth / 2;
                //YCenter = adornElement.ActualHeight / 2;
                XCenter = adornElement.ActualWidth / 2;
                YCenter = 7;

                r.Fill = _brush;
                _child = r;

            }
            else
                _child = adornElement;


        }


        private double _leftOffset;
        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value - XCenter;
                UpdatePosition();
            }
        }

        private double _topOffset;
        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = value - YCenter;

                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            AdornerLayer adorner = (AdornerLayer)this.Parent;
            if (adorner != null)
            {
                adorner.Update(this.AdornedElement);
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }


        protected override Size MeasureOverride(Size finalSize)
        {
            _child.Measure(finalSize);
            return _child.DesiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {

            _child.Arrange(new Rect(_child.DesiredSize));
            return finalSize;
        }

        public double scale = 1.0;
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();

            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }
    }
}
