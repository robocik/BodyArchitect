using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls;
using Coding4Fun.Phone.Controls.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace BodyArchitect.WP7.UserControls
{
    public partial class UpdateInfoControl
    {
        private const string Version = "4.0.0.1";

        public UpdateInfoControl()
        {
            InitializeComponent();
            //if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "pl")
            //{
            //    img1.Source = new BitmapImage(new Uri("/Images/Screen2_PL.JPG", UriKind.Relative));
            //    img2.Source = new BitmapImage(new Uri("/Images/Screen4_PL.JPG", UriKind.Relative));
            //    img3.Source = new BitmapImage(new Uri("/Images/Screen3_PL.JPG", UriKind.Relative));
            //}
        }

        public static void ShowUpdateInfo()
        {
            if (!ApplicationState.Current.IsOffline)
            {
                    string version = PhoneHelper.GetAppAttribute("Version");
                    if (version==Version && !ApplicationState.Current.UpdateVersions.Contains(version))
                    {
                        var ctrl = new UpdateInfoControl();
                        ctrl.ShowPopup();
                        ApplicationState.Current.UpdateVersions.Add(version);
                    }
            }
        }

        private void btnGoToWeb_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(Constants.VersionChangesVideoUrl);
            task.Show();
        }

        private void btnGoToWebPrices_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(Settings.ServerUrl + "V2/Payments.aspx?Token=" + ApplicationState.Current.SessionData.Token.SessionId);
            task.Show();
        }
    }
}
