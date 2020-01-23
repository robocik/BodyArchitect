using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class AnimatedRatingControl : UserControl
    {
        public AnimatedRatingControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RatingProperty =
                DependencyProperty.Register("Rating",
                typeof(double), typeof(AnimatedRatingControl),
                new PropertyMetadata(0.0, OnRatingPropertyChanged));

        public double Rating
        {
            get
            {
                return (double)GetValue(RatingProperty);
            }
            set { SetValue(RatingProperty, value); }
        }

        void Value_Changed(object sender,EventArgs e)
        {
            Rating = ratingCtrl.Value.HasValue?ratingCtrl.Value.Value:0.0;
        }
        static void OnRatingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var img = (AnimatedRatingControl)obj;
            img.ratingCtrl.Value = (double)args.NewValue;
        }
    }
}
