using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Portable;

namespace BodyArchitect.SplashScreen
{
    /// <summary>
    /// Interaction logic for SplashWindow1.xaml
    /// </summary>
    public partial class SplashWindow1
    {
        public SplashWindow1()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SplashScreenWindow_Loaded);
            MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(SplashScreenWindow_MouseDoubleClick);
            txtVersion.Text = string.Format(Strings.Splash_Version,Constants.Version);
        }

        void SplashScreenWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            companyBox.BeginAnimation();
        }

        void SplashScreenWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
            //progressIndicator.IsRunning = true;
            companyBox.BeginAnimation();
        }

        #region Public Properties & Methods


        public void SetStatus(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<string>(SetStatus), message);
            }
            else
            {
                this.StatusLabel.Text = message;
                
            }
        }

        public void SetProgressBar(bool visible, int max, int value)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action<bool, int, int>(SetProgressBar), visible, max, value);
            }
            else
            {
                //progressBarControl1.Visibility = visible?Visibility.Visible : Visibility.Collapsed;
                //progressBarControl1.Maximum = max;
                //progressBarControl1.Value = value;
            }
        }
        #endregion
    }
}
