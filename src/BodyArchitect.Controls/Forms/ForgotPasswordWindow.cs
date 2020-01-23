using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.ProgressIndicator;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Controls.Forms
{
    public partial class ForgotPasswordWindow : BaseWindow
    {
        private bool restorePassword;

        public ForgotPasswordWindow(bool restorePasswordMode=false)
        {
            InitializeComponent();
            restorePassword = restorePasswordMode;
            this.usrProgressIndicatorButtons1.OkButton.Enabled = false;
            if(!restorePassword)
            {
                Text = ApplicationStrings.ForgotPasswordWindow_SendActivationEmailTitle;
                lblDescription.Text = ApplicationStrings.ForgotPasswordWindow_SendActivationEmailDescription;
            }
        }
        
        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {
            try
            {
                ServiceManager.AccountOperation(textEdit1.Text,restorePassword?AccountOperationType.RestorePassword:AccountOperationType.SendActivationEmail);
                string message = restorePassword
                                     ? ApplicationStrings.ForgotPasswordWindow_PasswordChangedInfo
                                     :ApplicationStrings.ForgotPasswordWindow_ActivatedEmailSent;
                FMMessageBox.ShowInfo(message);
                ThreadSafeClose();
            }
            catch (ProfileIsNotActivatedException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorProfileIsActivated, ErrorWindow.MessageBox);
                }, null);
            }
            catch (EMailSendException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex,ApplicationStrings.ErrProblemWithSendingEmail,ErrorWindow.MessageBox);
                }, null);
            }
            catch (ObjectNotFoundException ex)
            {
                TasksManager.SetException(ex);
                this.SynchronizationContext.Send(delegate
                {
                    ExceptionHandler.Default.Process(ex, ApplicationStrings.ForgotPasswordWindow_ProfileNotFoundError, ErrorWindow.MessageBox);
                }, null);
            }
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            usrProgressIndicatorButtons1.OkButton.Enabled = textEdit1.Text.Length > 0;
        }

        private void usrProgressIndicatorButtons1_TaskProgressChanged(object sender, UserControls.TaskStateChangedEventArgs e)
        {
            bool startOperation = e.Context.State == OperationState.Started;
            baGroupControl1.Enabled = !startOperation;
        }
    }
}