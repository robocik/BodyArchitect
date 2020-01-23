using System;
using System.Reflection;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using BodyArchitect.Logger;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.Forms
{
    public partial class AboutWindow : XtraForm
    {
        public AboutWindow()
        {
            InitializeComponent();
            Text += " " + Constants.ApplicationName;
            lblVersionValue.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
            btnCancel.Text = ControlsStrings.CloseButton;
            lblWWW.Text = UserContext.Settings.WWW1;
            lblBetaVersion.Visible = Constants.IsBeta;
        }

        private void lblWWW_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(((LabelControl)sender).Text);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOpenWebBrowser, ErrorWindow.MessageBox);
            }
        }
    }
}
