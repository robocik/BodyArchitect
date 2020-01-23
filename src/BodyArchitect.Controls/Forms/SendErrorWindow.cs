using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Logger;
using DevExpress.XtraEditors;
using System.Net.Mail;

namespace BodyArchitect.Controls.Forms
{
    public partial class SendErrorWindow : DevExpress.XtraEditors.XtraForm
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
                return chkApplyAlways.Checked;
            }
        }

        private void SendErrorWindow_Load(object sender, EventArgs e)
        {
            Activate();
            BringToFront();
        }

    }
}