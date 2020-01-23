using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public class RectanglesControl:StackPanel
    {

        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set
            {
                SetValue(NumberProperty, value);
            }
        }


        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(int), typeof(RectanglesControl), new UIPropertyMetadata(-1, OnNumberChanged));

        private static void OnNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (RectanglesControl)d;
            ctrl.Children.Clear();
            int setNumbers = (int) e.NewValue+1;
            for (int i = 0; i < setNumbers; i++)
            {
                var rect = new Border();
                rect.BorderBrush = Brushes.Black;
                if (i > 0)
                {//for middle rectangles set half border
                    rect.BorderThickness = new Thickness(0,1,1,1);
                }
                else
                {
                    rect.BorderThickness = new Thickness(1);
                }
                
                rect.Width = rect.Height = 20;
                rect.Background = Brushes.White;
                ctrl.Children.Add(rect);
            }
            
        }
    }
}
