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
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : BaseWindow
    {
        private bool restorePassword;

        public ForgotPasswordWindow(bool restorePasswordMode = false)
        {
            InitializeComponent();

            restorePassword = restorePasswordMode;
            this.usrProgressIndicatorButtons1.OkButton.IsEnabled = false;
            if (!restorePassword)
            {
                Title = Strings.ForgotPasswordWindow_SendActivationEmailTitle;
                lblDescription.Text = Strings.ForgotPasswordWindow_SendActivationEmailDescription;
            }

        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {
            string username = null;
            SynchronizationContext.Send(delegate
                                            {
                                                username = txtUserNameEmail.Text;
                                            },null);
            try
            {
                ServiceManager.AccountOperation(username, restorePassword ? AccountOperationType.RestorePassword : AccountOperationType.SendActivationEmail);
                string message = restorePassword
                                     ? Strings.ForgotPasswordWindow_PasswordChangedInfo
                                     : Strings.ForgotPasswordWindow_ActivatedEmailSent;
                BAMessageBox.ShowInfo(message);
                ThreadSafeClose(true);
            }
            catch (ProfileIsNotActivatedException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrorProfileIsActivated, ErrorWindow.MessageBox);
                }, null);
            }
            catch (EMailSendException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrProblemWithSendingEmail, ErrorWindow.MessageBox);
                }, null);
            }
            catch (ObjectNotFoundException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, Strings.ForgotPasswordWindow_ProfileNotFoundError, ErrorWindow.MessageBox);
                }, null);
            }
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            usrProgressIndicatorButtons1.OkButton.IsEnabled = txtUserNameEmail.Text.Length > 0;
        }

        private void usrProgressIndicatorButtons1_TaskProgressChanged(object sender, TaskStateChangedEventArgs e)
        {
            bool startOperation = e.Context.State == OperationState.Started;
            baGroupControl1.IsEnabled = !startOperation;
        }
    }
}
