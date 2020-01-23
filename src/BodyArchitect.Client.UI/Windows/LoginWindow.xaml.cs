using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using OperationContext = BodyArchitect.Client.Common.OperationContext;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public event EventHandler LoginSuccessful;
        private SessionData loggedSessionData;

        public LoginWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(LoginWindow_Loaded);
            btnForceUpdate.Visibility = ShowForceUpdate?Visibility.Visible : Visibility.Collapsed;
        }

        void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings1.Default.AutoLoginUserName) && !string.IsNullOrEmpty(Settings1.Default.AutoLoginPassword))
            {
                txtUserName.Text = Settings1.Default.AutoLoginUserName;
                txtPassword.Password = Settings1.Default.AutoLoginPassword;
                chkAutologin.IsChecked = true;
                doLogin(true);
            }
            txtUserName.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            doLogin(false);
        }

        public SessionData LoggedSessionData
        {
            get { return loggedSessionData; }
        }

        public static bool ShowForceUpdate
        {
            get;
            set;
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {

        }

        void showUpdateInfoWindow()
        {
#if !DEBUG
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed && System.Deployment.Application.ApplicationDeployment.CurrentDeployment.IsFirstRun)

            {
                UpdateInfoWindow win = new UpdateInfoWindow();
                win.ShowDialog();
            }
#endif
        }

        private void doLogin(bool automaticLogin)
        {
            string username = txtUserName.Text;
            string password = txtPassword.Password;
            bool autoLogin = chkAutologin.IsChecked.Value;

            RunAsynchronousOperation(delegate(OperationContext context)
            {
                try
                {
                    var sessionData = UserContext.Current.Login(username, password, autoLogin, automaticLogin);
                    if (sessionData == null)
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            BAMessageBox.ShowError(EnumLocalizer.Default.GetStringsString("ErrorAuthenticationWrongCredentials"));
                        }));

                        return;
                    }
                    loggedSessionData = sessionData;

                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        if (LoginSuccessful != null)
                        {
                            LoginSuccessful(this, EventArgs.Empty);
                            foreach (var module in PluginsManager.Instance.Modules)
                            {
                                module.AfterUserLogin();
                            }
                            Close();

                            showUpdateInfoWindow();
                        }
                        else
                        {
                            DialogResult = true;
                            Close();
                        }
                    }));
                    
                    //ThreadSafeClose(true); 
                    
                }
                catch (SecurityAccessDeniedException ex)
                {
                    TasksManager.SetException(ex);
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("SecurityAccessDeniedException_LoginWindow_InvalidAPIKey"), ErrorWindow.EMailReport), Dispatcher);
                }
                catch (ProfileIsNotActivatedException ex)
                {
                    TasksManager.SetException(ex);
                    UIHelper.Invoke(delegate
                    {
                        //lblSendActivationEmail.Visible = true;//TODO:Finish
                        ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorProfileNotActivated"), ErrorWindow.MessageBox);
                    }, Dispatcher);
                }
                catch (ProfileDeletedException ex)
                {
                    TasksManager.SetException(ex);
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorCannotLoginToDeletedProfile"), ErrorWindow.MessageBox), Dispatcher);
                }
                catch (EndpointNotFoundException ex)
                {
                    TasksManager.SetException(ex);
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorConnectionProblem"), ErrorWindow.MessageBox), Dispatcher);

                }
                catch (TimeoutException ex)
                {
                    TasksManager.SetException(ex);

                    UIHelper.BeginInvoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorConnectionProblem"), ErrorWindow.MessageBox), Dispatcher);
                }
                catch (MaintenanceException ex)
                {
                    TasksManager.SetException(ex);
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorMaintenanceMode"), ErrorWindow.MessageBox), Dispatcher);

                }
                catch (DatabaseVersionException ex)
                {
                    TasksManager.SetException(ex);
                    UIHelper.BeginInvoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorOldVersionOfBodyArchitect"), ErrorWindow.MessageBox), Dispatcher);
                }
                catch (Exception ex)
                {
                    TasksManager.SetException(ex);
                    UIHelper.Invoke(() => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("ErrorUnhandledException"), ErrorWindow.MessageBox), Dispatcher);
                }

            }, AsyncOperationStateChange);
        }

        private void AsyncOperationStateChange(OperationContext state)
        {
            bool startLoginOperation = state.State == OperationState.Started;
            txtUserName.IsEnabled = !startLoginOperation;
            txtPassword.IsEnabled = !startLoginOperation;
            btnRegister.IsEnabled = !startLoginOperation;
            btnForgotPassword.IsEnabled = !startLoginOperation;
            btnLogin.IsEnabled = !startLoginOperation;
            progressIndicator.IsRunning = startLoginOperation;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            CreateProfileWindow dlg = new CreateProfileWindow();
            if (dlg.ShowDialog() == true)
            {
                loggedSessionData = dlg.CreatedSessionData;
                if (loggedSessionData != null)
                {//we allow to login without activation
                    UserContext.CreateUserContext(loggedSessionData);
                    if (LoginSuccessful != null)
                    {
                        LoginSuccessful(this, EventArgs.Empty);
                    }
                    Close();
                }
                else
                {
                    //lblSendActivationEmail.Visible = true;//TODO:Finish?

                    BAMessageBox.ShowInfo(EnumLocalizer.Default.GetStringsString("ErrorMustActivateProfile"));
                }
            }
        }

        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordWindow dlg = new ForgotPasswordWindow(true);
            dlg.ShowDialog();
        }

        private void btnForceUpdate_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", UserContext.Current.Settings.ClickOnceUrl);
            Application.Current.Shutdown();
            return;
        }
    }
}
