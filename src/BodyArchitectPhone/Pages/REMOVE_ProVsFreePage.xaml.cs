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
using BodyArchitect.WP7.UserControls;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class ProVsFreePage
    {
        public ProVsFreePage()
        {
            InitializeComponent();
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            BuyProgramMessageControl.ShowBuy();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            destroyPageAnimation();

            base.OnBackKeyPress(e);

        }

        void destroyPageAnimation()
        {
            Storyboard anim = new Storyboard();

            var x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g1);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.3);
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g2);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g3);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.3);
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g4);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g5);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.3);
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g6);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g7);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.3);
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g8);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g9);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.5);
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, btnBuy);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            

            anim.Begin();
            anim.Completed += delegate
            {
                LayoutRoot.Visibility = Visibility.Collapsed;
                anim.Stop();
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }

            };
        }

        void startPageAnimation()
        {
            Storyboard anim = new Storyboard();

            var x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g1);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.6);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g2);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.7);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g3);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.8);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g4);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(0.9);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g5);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(1);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g6);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(1.1);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g7);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(1.2);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g8);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            x.BeginTime = TimeSpan.FromSeconds(1.2);
            x.From = 0;
            x.To = 1;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, g9);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            anim.Begin();
            //anim.Completed += delegate
            //{
            //    LayoutRoot.Visibility = Visibility.Collapsed;
            //    anim.Stop();
            //    if (NavigationService.CanGoBack)
            //    {
            //        NavigationService.GoBack();
            //    }

            //};
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            startPageAnimation();
        }
    }
}