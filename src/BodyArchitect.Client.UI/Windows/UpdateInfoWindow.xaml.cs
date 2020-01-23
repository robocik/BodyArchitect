using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for UpdateInfoWindow.xaml
    /// </summary>
    public partial class UpdateInfoWindow : Window
    {
        public UpdateInfoWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UpdateInfoWindow_Loaded);
        }

        string getFeedUrl(string baseUrl)
        {
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "pl")
            {
                return string.Format(baseUrl, "_pl");
            }
            return string.Format(baseUrl, "");
        }

        void UpdateInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Navigate( getFeedUrl(Settings.ApplicationSettings.ServerUrl + @"V2\UpdateInfo\PC\UpdateInfo{0}.htm"));
        }
    }
}
