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
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace BodyArchitect.WP7.Pages
{
    public partial class MoreFeaturePage : AnimatedBasePage
    {
        public MoreFeaturePage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            lblDescription.MakeFormattedTextBlock(ApplicationStrings.MoreFeaturePage_Description);
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        private void GoToWebSite_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri =new Uri("http://bodyarchitectonline.com");
            task.Show();
        }

        private void GoToTutorialSite_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(Constants.TutorialUrl);
            task.Show();
        }
    }
}