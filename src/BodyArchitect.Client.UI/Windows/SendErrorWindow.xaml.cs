using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Logger;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for SendErrorWindow.xaml
    /// </summary>
    public partial class SendErrorWindow : Window
    {
        public SendErrorWindow()
        {
            InitializeComponent();
        }

        public void Fill(Exception exception)
        {
            txtSubject.Text = exception.Message;
            txtContent.Text = exception.GetExceptionString();
        }

        public bool ApplyAlways
        {
            get
            {
                return chkApplyAlways.IsChecked.Value;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
