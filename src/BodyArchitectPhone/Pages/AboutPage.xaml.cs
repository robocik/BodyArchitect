using System;
using System.Windows;
using System.Windows.Media.Animation;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.UserControls;
using Microsoft.Phone.Controls;
using Coding4Fun.Phone.Controls.Data;
using Microsoft.Phone.Tasks;

namespace BodyArchitect.WP7.Pages
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            tbVersion.Text =string.Format(ApplicationStrings.AboutPage_Version, PhoneHelper.GetAppAttribute("Version"));
        }


        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://bodyarchitectonline.com");
            task.Show();
        }

        private void hlRateAndReview_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }

        private void hlEmail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.To = "admin@bodyarchitectonline.com";
            task.Show();
        }

        void destroyPageAnimation()
        {
            Storyboard anim = new Storyboard();

            var x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 1;
            x.To = 0;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, LayoutRoot);
            Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 0;
            x.To = -500;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, versionTrans);
            Storyboard.SetTargetProperty(x, new PropertyPath("X"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.6));
            x.From = 0;
            x.To = 500;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, linkTrans);
            Storyboard.SetTargetProperty(x, new PropertyPath("Y"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 0;
            x.To = -1000;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, linkReviewTrans);
            Storyboard.SetTargetProperty(x, new PropertyPath("Y"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.4));
            x.From = 0;
            x.To = -1000;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, linkEmailTran);
            Storyboard.SetTargetProperty(x, new PropertyPath("Y"));

            x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            x.From = 0;
            x.To = 500;
            anim.Children.Add(x);
            Storyboard.SetTarget(x, linkFacebookTrans);
            Storyboard.SetTargetProperty(x, new PropertyPath("X"));

           
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

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            e.Cancel = true;
            destroyPageAnimation();
        }

        private void hlFacebook_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            //task.Uri = new Uri("https://www.facebook.com/pages/BodyArchitect/192639960758583");
            task.Uri = new Uri("https://www.facebook.com/BodyArchitectOnline");
        
            task.Show();
        }
    }
}