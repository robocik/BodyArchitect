using System;
using System.Deployment.Application;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Common;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;


namespace BodyArchitect.Controls.Forms
{
    public partial class LoginWindow : BaseWindow
    {
        private SessionData loggedSessionData;

        public LoginWindow()
        {
            InitializeComponent();
            lblForceUpdate.Visible = ShowForceUpdate;
            lblConnectionSettings.Text = ApplicationStrings.LoginWindow_ShowConnectionSettings;
        }

        public static bool ShowForceUpdate
        {
            get; set;
        }

        public SessionData LoggedSessionData
        {
            get { return loggedSessionData; }
        }

        private void AsyncOperationStateChange(OperationContext state)
        {
            bool startLoginOperation = state.State == OperationState.Started;
            txtName.Enabled = !startLoginOperation;
            txtPassword.Enabled = !startLoginOperation;
            lblRegister.Enabled = !startLoginOperation;
            lblForgotPassword.Enabled = !startLoginOperation;
            btnLogin.Enabled = !startLoginOperation;
            if (startLoginOperation)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
            progressIndicator1.Visible = startLoginOperation;
        }

    
        private void btnLogin_Click(object sender, EventArgs e)
        {
            doLogin(false);
        }

        private void doLogin(bool automaticLogin)
        {
            RunAsynchronousOperation(delegate(OperationContext context)
                                         {
                                             try
                                             {
                                                 var sessionData = UserContext.Login(txtName.Text, txtPassword.Text, chkAutologin.Checked, automaticLogin);
                                                 if (sessionData == null)
                                                 {
                                                     this.SynchronizationContext.Send(delegate
                                                     {
                                                         FMMessageBox.ShowError(ApplicationStrings.ErrorAuthenticationWrongCredentials);
                                                     }, null);
                                                     
                                                     return;
                                                 }
                                                 loggedSessionData = sessionData;
                                                 ThreadSafeClose();
                                             }
                                             catch (ProfileIsNotActivatedException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(delegate
                                                                                      {
                                                     lblSendActivationEmail.Visible= true;
                                                     ExceptionHandler.Default.Process(ex,ApplicationStrings.ErrorProfileNotActivated, ErrorWindow.MessageBox);
                                                 }, null);
                                             }
                                             catch (ProfileDeletedException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(delegate
                                                                                      {
                                                                                          ExceptionHandler.Default.Process(ex,ApplicationStrings.ErrorCannotLoginToDeletedProfile,ErrorWindow.MessageBox);
                                                                                      }, null);
                                             }
                                             catch (EndpointNotFoundException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorConnectionProblem, ErrorWindow.MessageBox), null);

                                             }
                                             catch (TimeoutException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorConnectionProblem, ErrorWindow.MessageBox), null);
                                             }
                                             catch (MaintenanceException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorMaintenanceMode, ErrorWindow.MessageBox), null);

                                             }
                                             catch (DatabaseVersionException ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(delegate
                                                                                      {
                                                                                          lblForceUpdate.Visible = true;
                                                     ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorOldVersionOfBodyArchitect, ErrorWindow.MessageBox);
                                                 }, null);
                                             }
                                             catch(Exception ex)
                                             {
                                                 TasksManager.SetException(ex);
                                                 this.SynchronizationContext.Send(delegate
                                                 {
                                                     ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorUnhandledException, ErrorWindow.MessageBox);
                                                 }, null);
                                             }
                            
                                         }, AsyncOperationStateChange);
        }

        private void LoginWindow_Load(object sender, EventArgs e)
        {

            chkEnable100Continue.Checked = Settings1.Default.Expect100Continue;
            if(!string.IsNullOrEmpty(Settings1.Default.AutoLoginUserName) && !string.IsNullOrEmpty(Settings1.Default.AutoLoginPassword))
            {
                txtName.Text = Settings1.Default.AutoLoginUserName;
                txtPassword.Text = Settings1.Default.AutoLoginPassword;
                doLogin(true);
            }
            txtName.Focus();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            btnLogin.Enabled = txtName.Text.Length > 0 && txtPassword.Text.Length > 0;
        }

        private void lblRegister_Click(object sender, EventArgs e)
        {
            CreateProfileWindow dlg = new CreateProfileWindow();
            if(dlg.ShowDialog(this)==DialogResult.OK)
            {
                loggedSessionData = dlg.CreatedSessionData;
                if (loggedSessionData != null)
                {//we allow to login without activation
                    UserContext.CreateUserContext(loggedSessionData);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    lblSendActivationEmail.Visible = true;
                    FMMessageBox.ShowInfo(ApplicationStrings.ErrorMustActivateProfile);
                }
            }
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {

        }

        private void lblForgotPassword_Click(object sender, EventArgs e)
        {
            ForgotPasswordWindow dlg = new ForgotPasswordWindow(true);
            dlg.ShowDialog(this);
        }

        private void lblForceUpdate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", UserContext.Settings.ClickOnceUrl);
            System.Windows.Forms.Application.Exit();
            return;
        }

        private void lblSendActivationEmail_Click(object sender, EventArgs e)
        {
            ForgotPasswordWindow dlg = new ForgotPasswordWindow();
            dlg.ShowDialog(this);
        }


        private void lblConnectionSettings_Click_1(object sender, EventArgs e)
        {
            if (splitContainerControl1.PanelVisibility==SplitPanelVisibility.Both)
            {
                lblConnectionSettings.Text = ApplicationStrings.LoginWindow_ShowConnectionSettings;
                splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
                Height -= 100;
            }
            else
            {
                lblConnectionSettings.Text = ApplicationStrings.LoginWindow_HideConnectionSettings;
                splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
                Height += 100;    
            }
            
        }

        private void chkEnable100Continue_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.Expect100Continue = chkEnable100Continue.Checked;
            ServicePointManager.Expect100Continue = Settings1.Default.Expect100Continue;
        }
    }
}