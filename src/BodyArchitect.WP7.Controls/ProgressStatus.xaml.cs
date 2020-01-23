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
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Controls
{
    public partial class ProgressStatus : UserControl
    {
        public ProgressStatus()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
            
        }

        public void ShowProgress(bool show,string status,bool forAppBarAlso=true,bool lockScreen=true)
        {
            if (Dispatcher.CheckAccess())
            {
                showProgressImplementation(show, status, forAppBarAlso, lockScreen);
            }
            else
            {
                Dispatcher.BeginInvoke(delegate
                {
                    showProgressImplementation(show, status, forAppBarAlso, lockScreen);
                });
            }
        }

        private void showProgressImplementation(bool show, string status,bool forAppBarAlso,bool lockScreen)
        {
            
            lblStatus.Foreground = Foreground;
            progressBar.ShowProgress(show, forAppBarAlso);
            if (lockScreen)
            {
                var page = progressBar.GetParent<PhoneApplicationPage>();
                if (page != null)
                {
                    page.IsHitTestVisible = !show;
                }
            }
            lblStatus.Text = status;
            Visibility = show ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            if (show)
            {
                animation.Begin();
            }
        }

        public void ShowProgress(bool show)
        {
            ShowProgress(show, string.Empty);
        }

        
        public bool IsOperationStarted
        {
            get
            {
                return progressBar.Visibility == Visibility.Visible;
            }
        }
    }
}
