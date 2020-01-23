using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Client.WCF;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Cache;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Coding4Fun.Phone.Controls;
using ImageTools;
using ImageTools.IO;
using ImageTools.IO.Bmp;
using ImageTools.IO.Gif;
using ImageTools.IO.Jpeg;
using ImageTools.IO.Png;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.UserControls
{
    public partial class MyProfileControl : UserControl
    {
        //private ChangeStatusControl addCommentCtrl = new ChangeStatusControl();
        private ProgressStatus progressBar;
        
        public MyProfileControl()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MyProfileControl_Loaded);

        }

        void MyProfileControl_Loaded(object sender, RoutedEventArgs e)
        {
            BuildApplicationBar();
        }


        public void Fill(ProfileInformationDTO profileInfo)
        {
            
            updateOfflineGuid();
            if (profileInfo != null)
            {
                DataContext = new MyProfileViewModel(profileInfo);
                //retrieve latest messages
                ((MyProfileViewModel)DataContext).LoadMessages();
                //lblPoints.Text = profileInfo.Licence.BAPoints.ToString();
                ctrlAwards.User = profileInfo.User;
                fillImage();
            }
            else
            {
                DataContext = null;
            }
        }

        void fillImage()
        {
            profileImage.Fill(ApplicationState.Current.SessionData.Profile.Picture);
        }


        public void SetProgressBar(ProgressStatus performanceProgressBar)
        {
            this.progressBar = performanceProgressBar;
        }

        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            if(ApplicationState.Current.IsOffline)
            {
                return;
            }
            ApplicationState.Current.EditProfileInfo = ApplicationState.Current.ProfileInfo.Copy();
            if(ApplicationState.Current.EditProfileInfo.Wymiary==null)
            {
                ApplicationState.Current.EditProfileInfo.Wymiary=new WymiaryDTO();
                ApplicationState.Current.EditProfileInfo.Wymiary.Time.DateTime = DateTime.Now;
            }
            var page=this.GetParent<MainPage>();
            page.Navigate("/Pages/ProfilePage.xaml");
        }

        public void BuildApplicationBar()
        {
            try
            {
                var page = this.GetParent<MainPage>();
                if (page != null && page.mainPanorama.SelectedIndex <= 0 && ApplicationState.Current != null)
                {

                    page.ApplicationBar = ExtensionMethods.CreateApplicationBar();
                    page.ApplicationBar.Opacity = .9;
                    page.ApplicationBar.Mode = ApplicationBarMode.Minimized;

                    if (!ApplicationState.Current.IsOffline)
                    {
                        ApplicationBarMenuItem menuItem = new ApplicationBarMenuItem(ApplicationStrings.AppBarButton_Refresh);
                        menuItem.Click += new EventHandler(mnuRefresh_Click);
                        page.ApplicationBar.MenuItems.Add(menuItem);
                    }
#if !DEBUG
                if (ApplicationState.Current.IsOffline)
#endif
                    {

                        var mnuOfflineMode = new ApplicationBarMenuItem(ApplicationState.Current.IsOffline
                                                           ? ApplicationStrings.MyProfileControl_GoOnline
                                                           : ApplicationStrings.MyProfileControl_GoOffline);
                        mnuOfflineMode.Click += new EventHandler(mnuOfflineMode_Click);
                        page.ApplicationBar.MenuItems.Add(mnuOfflineMode);
                    }

                    if (!ApplicationState.Current.IsOffline)
                    {
                        var mnuStatus = new ApplicationBarMenuItem(ApplicationStrings.MainPage_Status);
                        mnuStatus.Click += new EventHandler(mnuStatus_Click);
                        page.ApplicationBar.MenuItems.Add(mnuStatus);

                        var mnuLogout = new ApplicationBarMenuItem(ApplicationStrings.MainPage_LogoutLink);
                        mnuLogout.Click += new EventHandler(mnuLogout_Click);
                        page.ApplicationBar.MenuItems.Add(mnuLogout);


                    }
                }
            }
            catch (Exception)
            {
                //exception occures when we press back button very fast. for example we are in profile edit page and now we start pressing back many times to close application.
                //IN this case when applicaion is closed this exception occures. It doesn't have any bad impact. so we covert it in this catch block
            }
            
        }

        public void SaveState(IDictionary<string, object> state)
        {
            state["StatusDlg"] = statusPrompt != null;
            if (statusPrompt != null)
            {
                var ctrl = (ChangeStatusControl) statusPrompt.Body;
                ctrl.Save(state);
            }
        }

        public void RestoreState(IDictionary<string, object> state)
        {
            StateHelper helper = new StateHelper(state);

            if(helper.GetValue<bool>("StatusDlg",false))
            {
                showChangeStatusDlg(true, state);
            }
            
        }

        void mnuStatus_Click(object sender, EventArgs e)
        {
            showChangeStatusDlg(false, null);
        }

        private MessagePrompt statusPrompt;

        private void showChangeStatusDlg(bool afterTombstone, IDictionary<string, object> state)
        {
            var viewModel = (MyProfileViewModel) DataContext;
            var addCommentCtrl=new ChangeStatusControl();
            if (state != null)
            {
                addCommentCtrl.Restore(state);
            }
            addCommentCtrl.Fill(progressBar, viewModel.ProfileInfo.User.Statistics.Status.Status, afterTombstone);
            statusPrompt=addCommentCtrl.ShowPopup(messagePrompt =>
                                         {
                                             messagePrompt.IsCancelVisible = true;
                                             //ctrl.OperationCompleted += (h1, h2) =>
                                             //                               {
                                             //                                   viewModel.Status = ctrl.Comment;
                                             //                               };
                                             messagePrompt.Completed += (a1, s1) =>
                                                                            {

                                                                                if (s1.PopUpResult == PopUpResult.Ok)
                                                                                {
                                                                                    addCommentCtrl.ChangeStatus();
                                                                                    viewModel.Status = addCommentCtrl.Comment;
                                                                                }
                                                                                if (statusPrompt == messagePrompt)
                                                                                {
                                                                                    statusPrompt = null;
                                                                                }
                                                                            };
                                         }, this);
        }

        void mnuOfflineMode_Click(object sender, EventArgs e)
        {
            if (ApplicationState.Current.IsOffline)
            {
                BuildApplicationBar();
                login();
            }
            else
            {
                ApplicationState.Current.SaveState(false);
                ApplicationState.GoToOfflineMode();
            }
            updateOfflineGuid();
        }

        private void mnuLogout_Click(object sender, EventArgs e)
        {
            var page = this.GetParent<MainPage>();



            var m = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
            {
                client1.LogoutAsync(ApplicationState.Current.SessionData.Token);
                client1.WP7UnregisterAsync(Settings.ClientInstanceId.ToString());
            });

            m.Run();
            
            //var service = ApplicationState.CreateService();
            //if (!ApplicationState.Current.IsOffline)
            //{
            //    service.LogoutAsync(ApplicationState.Current.SessionData.Token);
            //    service.WP7UnregisterAsync(Settings.ClientInstanceId.ToString());
            //}
#if DEBUG
            ApplicationState.Current.SaveState(false);
#endif
            //remove phone reminders
            ApplicationState.Current.Cache.Reminders.RemoveAllReminders();
            Settings.UserName = null;
            Settings.Password = null;

            ApplicationState.Current.SessionData = null;
            ApplicationState.Current.TempUserName = null;
            ApplicationState.Current.TempPassword = null;
            ApplicationState.Current = null;
            //TODO:uncoment this in release
#if RELEASE
            ApplicationState.ClearOffline();
#endif
            PhoneApplicationService.Current.State["ApplicationState"] = null;
            IsolatedStorageSettings.ApplicationSettings.Save();
            page.ApplicationBar = null;
            page.ShowLogin(true);
        }

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressRetrieveProfile);
            ((MyProfileViewModel)DataContext).RefreshMessages();
            var m = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
            {
                GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
                client1.GetProfileInformationAsync(ApplicationState.Current.SessionData.Token, criteria);
                client1.GetProfileInformationCompleted -= operationCompleted;
                client1.GetProfileInformationCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, a) =>
            {
                progressBar.ShowProgress(false);
                if (a.Result.Result != null)
                {
                    ApplicationState.Current.ProfileInfo = a.Result.Result;
                    DataContext = new MyProfileViewModel(ApplicationState.Current.ProfileInfo);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.MyProfileControl_CantRetrieveProfileInfo_ErrMsg);
                }
            };

            if (!m.Run())
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

 

        //private void offlineMode_Click(object sender, RoutedEventArgs e)
        //{
        //    if(ApplicationState.Current.IsOffline)
        //    {
        //        hlOfflineMode.IsEnabled = false;
        //        login();
        //    }
        //    else
        //    {
        //        //ApplicationState.Current.SaveState();
        //        ApplicationState.GoToOfflineMode();
        //    }
        //    updateOfflineGuid();
        //}

        void updateOfflineGuid()
        {
            if (ApplicationState.Current==null)
            {
                return;
            }
            //if (mnuLogout != null)
            //{
            //    mnuLogout.IsEnabled = ApplicationState.Current.IsOffline;
            //}
            var count = ApplicationState.Current.MyDays.SelectMany(x=>x.Value.GetLocalModifiedEntries()).Count();
            lnkSynchronize.Visibility = count==0 || ApplicationState.Current.IsOffline ? Visibility.Collapsed : Visibility.Visible;
            lblOffline.Visibility = ApplicationState.Current.IsOffline
                                        ? System.Windows.Visibility.Visible
                                        : System.Windows.Visibility.Collapsed;
            pnlButtons.Visibility = !ApplicationState.Current.IsOffline
                                        ? System.Windows.Visibility.Visible
                                        : System.Windows.Visibility.Collapsed;
            BuildApplicationBar();
            //hlOfflineMode.Content = ApplicationState.Current.IsOffline ? ApplicationStrings.MyProfileControl_GoOnline : ApplicationStrings.MyProfileControl_GoOffline;

//            hlOfflineMode.Visibility = ApplicationState.Current.IsOffline ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
//#if DEBUG 
//            hlOfflineMode.Visibility = System.Windows.Visibility.Visible;
//#endif
            //imgOnline.Source = new BitmapImage(new Uri(ApplicationState.Current.IsOffline ? "/Images/Disconnected.png" : "/Images/Connected.png", UriKind.Relative)); 
        }

        void login()
        {

            progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressAuthentication);
            ClientInformation info = Settings.GetClientInformation();




            var m = new ServiceManager<LoginCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<LoginCompletedEventArgs> operationCompleted)
            {

                client1.LoginAsync(info, ApplicationState.Current.TempUserName, ApplicationState.Current.TempPassword, new object());

                client1.LoginCompleted -= operationCompleted;
                client1.LoginCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, a) =>
            {
                if (a.Error != null)
                {
                    progressBar.ShowProgress(false);
                    //hlOfflineMode.IsEnabled = true;
                    BuildApplicationBar();
                    BAMessageBox.ShowError(ApplicationStrings.ErrDuringLogin);
                    return;
                }
                else if (a.Result != null)
                {
                    progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressRetrieveProfile);
                    var state = new ApplicationState();
                    state.SessionData = a.Result.Result;
                    state.TempUserName = ApplicationState.Current.TempUserName;
                    state.TempPassword = ApplicationState.Current.TempPassword;
                    state.SessionData.Token.Language = ApplicationState.CurrentServiceLanguage;
                    state.Cache = ApplicationState.Current.Cache.Copy();
                    state.MyDays = ApplicationState.Current.MyDays;


                    var m1 = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
                    {

                        client1.GetProfileInformationAsync(state.SessionData.Token, new GetProfileInformationCriteria());

                        client1.GetProfileInformationCompleted -= operationCompleted;
                        client1.GetProfileInformationCompleted += operationCompleted;

                    });
                    m1.OperationCompleted += (s1, a1) =>
                                                {
                                                    if (a.Error != null)
                                                    {
                                                        //hlOfflineMode.IsEnabled = true;
                                                        BuildApplicationBar();
                                                        progressBar.ShowProgress(false);
                                                        BAMessageBox.ShowError(ApplicationStrings.ErrCantRetrieveProfileInfo);
                                                        return;
                                                    }
                                                    else if (a1.Result != null)
                                                    {
                                                        state.ProfileInfo = a1.Result.Result;
                                                        state.ProfileInfo.FavoriteUsers.Sort(delegate(UserSearchDTO u1, UserSearchDTO u2)
                                                        {
                                                            return u1.UserName.CompareTo(u2.UserName);
                                                        });
                                                        state.ProfileInfo.Friends.Sort(delegate(UserSearchDTO u1, UserSearchDTO u2)
                                                        {
                                                            return u1.UserName.CompareTo(u2.UserName);
                                                        });
                                                        state.CurrentBrowsingTrainingDays = null;
                                                        state.IsOffline = false;
                                                        ApplicationState.Current = state;
                                                        ApplicationState.Current.Cache.ClearAfterLogin();
                                                        Fill(ApplicationState.Current.ProfileInfo);
                                                        progressBar.ShowProgress(false);
                                                        //hlOfflineMode.IsEnabled = true;
                                                        BuildApplicationBar();
                                                        ((MyProfileViewModel)DataContext).RefreshMessages();
                                                    }
                                                };
                    m1.Run(true);

                }
                else
                {

                    //hlOfflineMode.IsEnabled = true;
                    BuildApplicationBar();
                    progressBar.ShowProgress(false);
                    BAMessageBox.ShowError(ApplicationStrings.ErrUserOrPasswordNotValid);

                }
            };

            if (!m.Run(true))
            {
                BuildApplicationBar();
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }


            
        }

        private void hlStatistics_Click(object sender, RoutedEventArgs e)
        {
            this.GetParent<PhoneApplicationPage>().Navigate("/Pages/StatisticsPage.xaml");
        }


    }
}
