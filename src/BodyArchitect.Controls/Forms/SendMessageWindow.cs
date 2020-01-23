using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls.ProgressIndicator;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Controls.Forms
{
    public partial class SendMessageWindow : BaseWindow
    {
        public SendMessageWindow()
        {
            InitializeComponent();
            usrProgressIndicatorButtons1.OkButton.Image = Icons.MessageSend;
            this.usrProgressIndicatorButtons1.OkButton.Enabled = false;
        }

        public void Fill(MessageDTO message)
        {
            usrMessageView1.Fill(message);
        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {
            try
            {
                var msg = usrMessageView1.SaveChanges();
                ServiceManager.SendMessage(msg);
                ThreadSafeClose();
            }
            catch (ValidationException validException)
            {
                TasksManager.SetException(validException);
                this.SynchronizationContext.Send(delegate
                {
                    FMMessageBox.ShowValidationError(validException.Results);
                }, null);
            }
            

        }

        private void usrMessageView1_ControlValidated(object sender, ControlValidatedEventArgs e)
        {
            this.usrProgressIndicatorButtons1.OkButton.Enabled = e.IsValid;
        }
    }
}
