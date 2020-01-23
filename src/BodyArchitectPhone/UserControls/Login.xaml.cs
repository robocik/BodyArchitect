using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.Cache;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using BodyArchitect.WP7;
using BodyArchitect.WP7.Client.WCF;
using BodyArchitect.WP7.Controls;
using BodyArchitectCustom;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.UserControls
{
    public partial class Login
    {
        public event EventHandler LoggingChanged;
        private ProgressStatus progressBar;
        private PushNotificationsHelper pushNotification;
        private OperationToken currentOperation;
        
        public Login()
        {
            InitializeComponent();

#if DEBUG
            lpEndPointSelector.Visibility = Visibility.Visible;
            emptySpace.Visibility = Visibility.Collapsed;
#else
            emptySpace.Visibility = Visibility.Visible;
            lpEndPointSelector.Visibility = Visibility.Collapsed;
#endif
            pushNotification=new PushNotificationsHelper();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("EndPoint"))
            {
                string endPoint = (string)IsolatedStorageSettings.ApplicationSettings["EndPoint"];
                
                if (endPoint == "Production")
                {
                    lpEndPointSelector.SelectedIndex = 0;
                }
                else if (endPoint == "TestWeb")
                {
                    lpEndPointSelector.SelectedIndex = 1;
                }
                else if (endPoint == "Local")
                {
                    lpEndPointSelector.SelectedIndex = 2;
                }
                else if (endPoint == "Local2")
                {
                    lpEndPointSelector.SelectedIndex = 3;
                }
            }
            else
            {
                lpEndPointSelector.SelectedIndex = 0;
            }
        }


        private bool IsLogining { get; set; }

        public ApplicationBar ApplicationBar
        {
            get { return (ApplicationBar)this.GetParent<PhoneApplicationPage>().ApplicationBar; }
            set { this.GetParent<PhoneApplicationPage>().ApplicationBar=value; }
        }
        
        void buildApplicationBar()
        {
            if (ApplicationBar != null || companyBox.Visibility==Visibility.Visible)
            {
                return;
            }
            var appBar = ExtensionMethods.CreateApplicationBar();
            ApplicationBar = appBar;
            bool showCancel = IsLogining  ;
            var icon = new Uri(!showCancel ? "/Toolkit.Content/ApplicationBar.Check.png" : "/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative);
            ApplicationBarIconButton button1 = new ApplicationBarIconButton(icon);
            button1.Click += new EventHandler(btnLogin_Click);
            button1.Text = showCancel ? ApplicationStrings.CancelButton : ApplicationStrings.LoginButton;
            appBar.Buttons.Add(button1);

            ApplicationBarMenuItem registerMenu = new ApplicationBarMenuItem(ApplicationStrings.Login_CreateAccount);
            registerMenu.Click += new EventHandler(registerMenu_Click);
            appBar.MenuItems.Add(registerMenu);

            ApplicationBarMenuItem offlineMode = new ApplicationBarMenuItem(ApplicationStrings.Login_OfflineMode);
            offlineMode.Click += new EventHandler(offlineMode_Click);
            appBar.MenuItems.Add(offlineMode);

            ApplicationBarMenuItem forgotPassword = new ApplicationBarMenuItem(ApplicationStrings.Login_ForgotPassword);
            forgotPassword.Click += new EventHandler(forgotPassword_Click);
            appBar.MenuItems.Add(forgotPassword);
        }

        void forgotPassword_Click(object sender, EventArgs e)
        {
            ForgotPasswordControl.Show();
        }

        void offlineMode_Click(object sender, EventArgs e)
        {
            goToOffline(false);
        }

        private void goToOffline(bool automatic)
        {
            showProgress(true, true);
            progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressOfflineModeStart);
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 try
                                                 {
                                                     ApplicationState.GoToOfflineMode();

                                                     Deployment.Current.Dispatcher.BeginInvoke(delegate
                                                                                                   {
                                                                                                       progressBar.ShowProgress(false);
                                                                                                       
                                                                                                       onLoggingChanged();
                                                                                                       if (!Settings.InfoOfflineMode)
                                                                                                       {
                                                                                                           Settings.InfoOfflineMode = true;
                                                                                                           BAMessageBox.ShowInfo(ApplicationStrings.MessageOfflineModeDescription);
                                                                                                       }
                                                                                                   });
                                                 }
                                                 catch (InvalidOperationException ex)
                                                 {
                                                     Deployment.Current.Dispatcher.BeginInvoke(() => showProgress(false, true));
                                                     if (automatic)
                                                     {
                                                         BAMessageBox.ShowError(ApplicationStrings.ErrDuringLogin);
                                                     }
                                                     else
                                                     {
                                                         BAMessageBox.ShowWarning(ApplicationStrings.Login_ErrGoOffline_MustLoginFirst, true);     
                                                     }
                                                 }
                                                 catch
                                                 {
                                                     Deployment.Current.Dispatcher.BeginInvoke(() => showProgress(false, true));
                                                     if (automatic)
                                                     {
                                                         BAMessageBox.ShowError(ApplicationStrings.ErrDuringLogin);
                                                     }
                                                     else
                                                     {
                                                         BAMessageBox.ShowError(ApplicationStrings.Login_ErrGoOfflineMode);
                                                     }
                                                     
                                                 }

                
                                             });
        }

        private void onLoggingChanged()
        {
            if (LoggingChanged != null)
            {
                LoggingChanged(this, EventArgs.Empty);
            }
        }

        void registerMenu_Click(object sender, EventArgs e)
        {
            this.GetParent<PhoneApplicationPage>().Navigate("/Pages/CreateProfilePage.xaml");
        }

        public void SetLoginControls(string username,string password)
        {
            
            txtUserName.Text = username;
            txtPassword.Password = password;
        }

        
        async public Task LoginImplementation(string username,string password)
        {
            if (!progressBar.IsOperationStarted)
            {//we should perform login
                showProgress(true);
                IsLogining = true;
                var btnLogin = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                btnLogin.IsEnabled = true;
                ApplicationState.Current = null;

                progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressAuthentication);

                try
                {
                    currentOperation = new OperationToken();
                    var sessionData = await BAService.LoginAsync(txtUserName.Text, password.ToSHA1Hash());
                    if (sessionData == null)
                    {
                        Settings.UserName = null;
                        Settings.Password = null;
                        IsLogining = false;
                        showProgress(false);
                        BAMessageBox.ShowError(ApplicationStrings.ErrUserOrPasswordNotValid);
                        return;
                    }
                    if (currentOperation != null && currentOperation.Cancelled)
                    {
                        IsLogining = false;
                        return;
                    }


                    progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressRetrieveProfile);
                    btnLogin.IsEnabled = true;

                    if (Settings.LiveTileEnabled && Settings.InitialAsk)
                    {
                        try
                        {
                            pushNotification.RegisterDevice(sessionData.Profile.GlobalId);
                        }
                        catch
                        {
                        }
                    }

                    var state = ApplicationState.LoadState();
                    if (state == null)
                    {
                        state = new ApplicationState();
                    }
                    else if (state.TempUserName != username)
                    {
                        //if we log as a different user, we should't use cache
                        ApplicationState.ClearOffline();
                        state = new ApplicationState();
                    }

                    state.IsOffline = false;
                    ApplicationState.Current = state;
                    ApplicationState.Current.SessionData =sessionData;
                    ApplicationState.Current.TempUserName = username;
                    ApplicationState.Current.TempPassword = password.ToSHA1Hash();
                    ApplicationState.Current.SessionData.Token.Language = ApplicationState.CurrentServiceLanguage;
                    
                    Settings.UserName = username;
                    Settings.Password = password;

                    try
                    {
                        await getProfileInformation();
                    }
                    catch (Exception)
                    {
                        showProgress(false);
                        IsLogining = false;
                        BAMessageBox.ShowError(ApplicationStrings.ErrCantRetrieveProfileInfo);
                    }
                }
                //catch (NetworkException)
                //{
                //    IsLogining = false;
                //    showProgress(false);
                //    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                //}
                catch (DatabaseVersionException)
                {
                    IsLogining = false;
                    showProgress(false);
                    BAMessageBox.ShowError(ApplicationStrings.ErrOldApplicationVersion);
                }
                catch (Exception ex)
                {
                    showProgress(false);
                    IsLogining = false;
                    goToOffline(true);
                }
            }
            else
            {
                if(currentOperation!=null)
                {
                    currentOperation.Cancel();
                    currentOperation = null;
                }
                showProgress(false);
            }
        }

        async private Task getProfileInformation()
        {
            currentOperation = new OperationToken();
            var profileInfo = await BAService.GetProfileInformationAsync(new GetProfileInformationCriteria());
            if (currentOperation != null && currentOperation.Cancelled)
            {
                showProgress(false);
                IsLogining = false;
                return;
            }

            ApplicationState.Current.ProfileInfo = profileInfo;

            ApplicationState.Current.Cache.Reminders.EnsureReminders();

            ApplicationState.Current.ProfileInfo.FavoriteUsers.Sort
                (delegate(UserSearchDTO u1, UserSearchDTO u2)
                     {
                         return u1.UserName.CompareTo(u2.UserName);
                     });
            ApplicationState.Current.ProfileInfo.Friends.Sort(
                delegate(UserSearchDTO u1, UserSearchDTO u2)
                {
                    return u1.UserName.CompareTo(u2.UserName);
                });
            if (ApplicationState.Current.MyDays == null)
            {
                ApplicationState.Current.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            }
            ApplicationState.Current.Cache.ClearAfterLogin();
            ApplicationState.Current.CurrentBrowsingTrainingDays =
                null;
            ApplicationState.Current.ClearTrainingDays();
            showProgress(false, false);
            IsLogining = false;
            onLoggingChanged();
            
            //var m = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
            //{

            //    currentOperation = new OperationToken();
            //    client1.GetProfileInformationAsync(ApplicationState.Current.SessionData.Token, new GetProfileInformationCriteria());

            //    client1.GetProfileInformationCompleted -= operationCompleted;
            //    client1.GetProfileInformationCompleted += operationCompleted;

            //});
            //m.OperationCompleted += (s, a1) =>
            //{
            //    if (a1.Error !=null)
            //          {
            //              showProgress(false);
            //              IsLogining=false;
            //              BAMessageBox.ShowError(ApplicationStrings.ErrCantRetrieveProfileInfo);
            //              return;
            //          }
            //          OperationToken op1 =(OperationToken)a.UserState;
            //          if (op1 !=null && op1.Cancelled)
            //          {
            //              showProgress(false);
            //              IsLogining=false;
            //              return;
            //          }

            //          else if (a1.Result != null)
            //          {
            //              //if (Settings.RefreshFrequencyDays == 0)
            //              //{
            //              //    //in trial/free mode we retrieve exercises and the rest stuff in every login
            //              //    ApplicationState.Current.Cache.Clear();
            //              //}

            //              ApplicationState.Current.ProfileInfo = a1.Result.Result;

            //              ApplicationState.Current.Cache.Reminders.EnsureReminders();

            //              ApplicationState.Current.ProfileInfo.FavoriteUsers.Sort
            //                  (delegate(UserSearchDTO u1, UserSearchDTO u2)
            //                       {
            //                           return u1.UserName.CompareTo(u2.UserName);
            //                       });
            //              ApplicationState.Current.ProfileInfo.Friends.Sort(
            //                  delegate(UserSearchDTO u1, UserSearchDTO u2)
            //                      {
            //                          return u1.UserName.CompareTo(u2.UserName);
            //                      });
            //              if (ApplicationState.Current.MyDays == null)
            //              {
            //                  //ApplicationState.Current.MyDays=new TrainingDaysHolder(ApplicationState.Current.SessionData.Profile);
            //                  ApplicationState.Current.MyDays =new Dictionary<CacheKey, TrainingDaysHolder>();
            //              }
            //              ApplicationState.Current.Cache.ClearAfterLogin();
            //              ApplicationState.Current.CurrentBrowsingTrainingDays =
            //                  null;
            //              ApplicationState.Current.ClearTrainingDays();
            //              showProgress(false, false);
            //              IsLogining = false;
            //              onLoggingChanged();
            //          }
            //};

            //if (!m.Run(true))
            //{
            //    IsLogining = false;
            //    showProgress(false);
            //    if (ApplicationState.Current.IsOffline)
            //    {
            //        BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
            //    }
            //    else
            //    {
            //        BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
            //    }
            //}


        }

        public void SetProgressBar(ProgressStatus performanceProgressBar)
        {
            this.progressBar = performanceProgressBar;
        }

        public bool Logged { get; set; }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(Settings.InitialAsk)
            {
                login();
                return;
            }

            var ctrl = new FeatureConfirmationControl();
            ctrl.ApplicationBar = ApplicationBar;
            ctrl.ShowPopup(messagePrompt=>
                               {
                                   messagePrompt.IsCancelVisible = false;
                                   messagePrompt.Completed += (a, s) =>
                                   {
                                       Settings.InitialAsk = true;
                                       ApplicationBar = ctrl.ApplicationBar;
                                       login();
                                   };
                               });
            
        }

        private void login()
        {
            companyBox.BeginAnimation();

            

#if !DEBUG

            IsolatedStorageSettings.ApplicationSettings["EndPoint"] = "Production";
#else
            ListPickerItem item = (ListPickerItem)lpEndPointSelector.SelectedItem;
            IsolatedStorageSettings.ApplicationSettings["EndPoint"] = (string)item.Tag; 
#endif


            LoginImplementation(txtUserName.Text, txtPassword.Password);
        }

        public void ShowLoginControls(bool show)
        {
            loginBox.Visibility = !show ? Visibility.Collapsed : Visibility.Visible;
            companyBox.Visibility = !show ? Visibility.Visible : Visibility.Collapsed;

            lpEndPointSelector.IsEnabled = show;
            txtUserName.IsEnabled = show;
            txtPassword.IsEnabled = show;
        }
        private void showProgress(bool show,bool forAppBarAlso=true)
        {
            ShowLoginControls(!show);

            progressBar.ShowProgress(show, string.Empty, forAppBarAlso);
            var btnLogin = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btnLogin.IsEnabled = true;
            btnLogin.Text = show ? ApplicationStrings.CancelButton : ApplicationStrings.LoginButton;
            btnLogin.IconUri = new Uri(!show ? "/Toolkit.Content/ApplicationBar.Check.png" : "/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative);

        }

        private void txtPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                login();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            ShowApplicationBar();
        }

        public void ShowApplicationBar()
        {
            if (ApplicationState.Current == null)
            {
                buildApplicationBar();
            }
            else
            {
                this.GetParent<MainPage>().BuildApplicationBar();
            }
        }

        private void lpEndPointSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lpEndPointSelector != null)
            {
                ListPickerItem item = (ListPickerItem) lpEndPointSelector.SelectedItem;
                IsolatedStorageSettings.ApplicationSettings["EndPoint"] = (string) item.Tag;
            }
        }
    }


}