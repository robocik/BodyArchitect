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
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Portable.Exceptions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for SendMessageWindow.xaml
    /// </summary>
    public partial class SendMessageWindow
    {
        private MessageDTO message;

        public SendMessageWindow()
        {
            InitializeComponent();

            BitmapImage img = "SendMessage.png".ToResourceUrl().ToBitmap();
            this.usrProgressIndicatorButtons1.OkButton.SetValue(ImageButtonExt.ImageProperty, img);

            //usrProgressIndicatorButtons1.OkButton.Image = Icons.MessageSend;
            this.usrProgressIndicatorButtons1.OkButton.IsEnabled = false;
        }

        public void Fill(MessageDTO message)
        {
            this.message = message;
            usrMessageView1.Fill(message);
        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {
            try
            {
                ServiceManager.SendMessage(message);
                ThreadSafeClose(true);
            }
            catch (ValidationException validException)
            {
                TasksManager.SetException(validException);
                this.SynchronizationContext.Send(delegate
                {
                    BAMessageBox.ShowValidationError(validException.Results);
                }, null);
            }


        }

        private void usrMessageView1_ControlValidated(object sender, ControlValidatedEventArgs e)
        {
            this.usrProgressIndicatorButtons1.OkButton.IsEnabled = e.IsValid;
        }
    }
}
