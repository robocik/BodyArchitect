using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Service.Client.WP7;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.UserControls
{
    public partial class FeatureConfirmationControl : UserControl
    {
        private bool isInitialized;

        public FeatureConfirmationControl()
        {
            InitializeComponent();

            tsSendUsageInfo.IsChecked = Settings.SendUsageData;
            tsSendCrashData.IsChecked = Settings.SendCrashData;
            tsEnablePushNotificaions.IsChecked = Settings.LiveTileEnabled;
            isInitialized = true;
        }

        private void Settings_Changed(object sender, RoutedEventArgs e)
        {
            if (isInitialized)
            {
                Settings.SendUsageData = tsSendUsageInfo.IsChecked.Value;
                Settings.SendCrashData = tsSendCrashData.IsChecked.Value;
                Settings.LiveTileEnabled = tsEnablePushNotificaions.IsChecked.Value;
            }
        }

        public ApplicationBar ApplicationBar { get; set; }
    }
}
