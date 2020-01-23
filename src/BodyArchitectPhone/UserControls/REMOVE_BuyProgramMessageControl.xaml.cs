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
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace BodyArchitect.WP7.UserControls
{
    public partial class BuyProgramMessageControl : UserControl
    {
        public BuyProgramMessageControl()
        {
            InitializeComponent();
        }


        public Button FillProVersion(string featureName,string desc,bool inPopup)
        {
            pnlPro.Visibility = Visibility.Visible;
            pnlPC.Visibility = Visibility.Collapsed;
            lblPro1.MakeFormattedTextBlock(desc);
            var btn = new Button();
            btn.Content = ApplicationStrings.BuyProgramMessageControl_ButtonBuy;
            lblFeatureName.Text = featureName;
            btn.Click += delegate
            {
                ShowBuy();
            };
            if(!inPopup)
            {
                this.panel.Children.Add(btn);
            }
            return btn;
        }
        public static void ShowPCVersion(string featureName,DependencyObject page )
        {
            var ctrl = new BuyProgramMessageControl();
            ctrl.pnlPro.Visibility = Visibility.Collapsed;
            ctrl.pnlPC.Visibility = Visibility.Visible;
            var popup = ctrl.ShowPopup(t =>
            {
                t.ActionPopUpButtons.Clear();
                Button btn = new Button();
                ctrl.lblFeatureName.Text = featureName;
                ctrl.lblPC1.MakeFormattedTextBlock(ApplicationStrings.BuyProgramMessageControl_lblPC1);
                ctrl.lblPC2.MakeFormattedTextBlock(ApplicationStrings.BuyProgramMessageControl_lblPC2);
                btn.Content = ApplicationStrings.BuyProgramMessageControl_ButtonBuy;
                btn.Click += delegate
                                 {
                                     ShowBuy();
                                 };
                t.ActionPopUpButtons.Add(btn);
                btn = new Button();
                btn.Content = ApplicationStrings.BuyProgramMessageControl_ButtonCancel;
                btn.Click += delegate
                {
                    t.Hide();
                };
                t.ActionPopUpButtons.Add(btn);
            },page);
        }

        public static void ShowBuy()
        {
#if TRIAL_SIMULATION && !RELEASE
            if(MessageBox.Show("Buy app?","BodyArchitect",MessageBoxButton.OKCancel)==MessageBoxResult.OK)
            {
                TrialManager.Buy();
                new TrialManager().DetermineIsTrail();
            }
            return;
#endif
            try
            {
                MarketplaceDetailTask task = new MarketplaceDetailTask();
                task.ContentIdentifier = Constants.MarketplaceId;
                task.Show();
            }
            catch (Exception)
            {
            }
            
        }

        public static void ShowProVersion(string featureName,DependencyObject page)
        {

            showProImplementation(featureName, ApplicationStrings.BuyProgramMessageControl_lblPC1, page);
        }

        static void showProImplementation(string title, string text, DependencyObject page)
        {
 
            var ctrl = new BuyProgramMessageControl();
            var popup=ctrl.ShowPopup(t=>
                               {
                                   t.ActionPopUpButtons.Clear();
                                   var btn = ctrl.FillProVersion(title, text, true);
                                   t.ActionPopUpButtons.Add(btn);
                                   btn = new Button();
                                   btn.Content = ApplicationStrings.BuyProgramMessageControl_ButtonCancel;
                                   btn.Click += delegate
                                   {
                                       t.Hide();
                                   };
                                   t.ActionPopUpButtons.Add(btn);
                               }, page);
        }

        public static void ShowTrialExpired()
        {
            showProImplementation(ApplicationStrings.TrialStatusExpired, ApplicationStrings.TrialExpiredDescription, null);
        }

        private void GoToWebSite_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.URL = "http://bodyarchitectonline.com";
            task.Show();
        }
    }
}
