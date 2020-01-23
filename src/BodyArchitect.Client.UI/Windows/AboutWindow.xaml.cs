using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BodyArchitect.Client.Common;
using BodyArchitect.Portable;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private bool isDestroying = false;

        public AboutWindow()
        {
            InitializeComponent();
            Closing += new System.ComponentModel.CancelEventHandler(AboutWindow_Closing);
            tbVersion.Text = Constants.Version;
        }

        void AboutWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isDestroying)
            {
                e.Cancel = true;
                isDestroying = true;
                destroyPageAnimation();
            }
        }

        private void Window_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void btnOpenWebSite_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenUrl("http://bodyarchitectonline.com");
        }

        private void btnEMailUs_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto:admin@bodyarchitectonline.com");
        }

        void destroyPageAnimation()
        {
            //TODO:Needs optimization. For now this anim Storyborad is only for Completed event. It doesn't change the position of elements. For this we invoke BeginAnimation for every DoubleAnimation
            Storyboard anim = new Storyboard();

            var x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, LayoutRoot);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            var translation = new TranslateTransform(0, 0);
            tbVersion.RenderTransform = translation;

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 0;
            x.To = -500;
            translation.BeginAnimation(TranslateTransform.XProperty, x);

            translation = new TranslateTransform(0, 0);
            btnWebSite.RenderTransform = translation;
            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.6));
            x.From = 0;
            x.To = 500;
            translation.BeginAnimation(TranslateTransform.YProperty, x);
            anim.Children.Add(x);
            Storyboard.SetTarget(x, translation);
            Storyboard.SetTargetProperty(x, new PropertyPath("Y"));


            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 0;
            x.To = -1000;
            translation = new TranslateTransform(0, 0);
            btnEmailUs.RenderTransform = translation;
            translation.BeginAnimation(TranslateTransform.YProperty, x);
            anim.Children.Add(x);
            Storyboard.SetTarget(x, translation);
            Storyboard.SetTargetProperty(x, new PropertyPath("Y"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 0;
            x.To = -1000;
            translation = new TranslateTransform(0, 0);
            btnFacebook.RenderTransform = translation;
            translation.BeginAnimation(TranslateTransform.YProperty, x);
            anim.Children.Add(x);
            Storyboard.SetTarget(x, translation);
            Storyboard.SetTargetProperty(x, new PropertyPath("Y"));

            anim.Completed += delegate
            {
                LayoutRoot.Visibility = Visibility.Collapsed;
                anim.Stop();
                Close();

            };
            anim.Begin();

        }

        private void btnOpenFacebook_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenUrl("http://www.facebook.com/pages/BodyArchitect/192639960758583");
        }
    }
}
