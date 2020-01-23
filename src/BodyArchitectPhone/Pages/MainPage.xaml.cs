using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using AdRotator;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Pages;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.Utils;
using BodyArchitectPhone;
using Coding4Fun.Phone.Controls;
using Coding4Fun.Phone.Controls.Data;
using Microsoft.Advertising;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;


namespace BodyArchitectCustom
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            loginCtrl.SetProgressBar(progressBar);
            myProfileCtrl.SetProgressBar(progressBar);
            todayCtrl.SetControls(progressBar);

            //set big live tile for wp 7.8 and 8.0
            ThreadPool.QueueUserWorkItem((a) =>
                                             {
                                                 LargeLiveTileHelper.UpdateFlipTile(null, null, null, null, null, new Uri("/", UriKind.Relative), null, null, null, new Uri("/Images/panoramaBackground.jpg", UriKind.RelativeOrAbsolute), null);
                                             });
            
            

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {


            
            var statPage = e.Content as StatisticsPage;
            if (statPage != null )
            {
                statPage.User = ApplicationState.Current.ProfileInfo.User;
            }
            //if (e.Content is EntryObjectPageBase)
            //{
            //    todayCtrl.PrepareTrainingDay();
            //}
             
            myProfileCtrl.SaveState(State);
            ApplicationState.OfflineModeChanged -= new EventHandler(ApplicationState_OfflineModeChanged);
            base.OnNavigatedFrom(e);
            
        }

        void ApplicationState_OfflineModeChanged(object sender, EventArgs e)
        {
            updateOfflineModeGui();
        }

        private void updateOfflineModeGui()
        {
            if (ApplicationState.Current != null)
            {
                pnlButtons.Visibility = ApplicationState.Current.IsOffline ? Visibility.Collapsed : Visibility.Visible;
                hkFeatured.Visibility = ApplicationState.Current.IsOffline ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
            ApplicationState.OfflineModeChanged += new EventHandler(ApplicationState_OfflineModeChanged);
            base.OnNavigatedTo(e);

            myProfileCtrl.RestoreState(State);
            hlRateMe.Visibility = Settings.RunsCount > Constants.ShowRateMeLinkAfterRuns ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            hlTutorial.Visibility=hlRateMe.Visibility==Visibility.Visible?Visibility.Collapsed : Visibility.Visible;

            if (ApplicationState.Current == null)
            {
                ShowLogin(true);

                if (!string.IsNullOrEmpty(Settings.UserName) && !string.IsNullOrEmpty(Settings.PasswordKey))
                {
                    loginCtrl.SetLoginControls(Settings.UserName, Settings.Password);
                    loginCtrl.LoginImplementation(Settings.UserName, Settings.Password);
                }
            }
            else
            {
                ShowLogin(false);
            }
            
            if (ApplicationState.Current != null)
            {
                myProfileCtrl.Fill(ApplicationState.Current.ProfileInfo);

                todayCtrl.Fill(DateTime.Now);

                
            }
            updateOfflineModeGui();
            
        }

        void checkCrashState()
        {
            if (ApplicationState.Current == null)
            {
                return;
            }
            bool crash = ApplicationState.Current.Crash;
            ApplicationState.Current.Crash = false;
            
            if (ApplicationState.Current.TrainingDay != null && crash )
            {
                if (BAMessageBox.Ask(ApplicationStrings.TrainingDaySelectorControl_QCrashSaverRestoreEntry) ==
                    MessageBoxResult.OK)
                {
                    LocalObjectKey id = ApplicationState.Current.CurrentEntryId;
                    Object currentEntry = ApplicationState.Current.TrainingDay.TrainingDay.GetEntry(id);
                    TrainingDaySelectorControl.GoToPage(currentEntry, this);
                    return;
                }
                
            }
            ApplicationState.Current.ResetCurrents();
        }

        public void ShowLogin(bool show)
        {
            ((App)App.Current).EnsureAdVisible(null);
            LayoutRoot.Background = show ? (Brush)new ImageBrush() { ImageSource = new BitmapImage(new Uri("/SplashScreenImage.jpg",UriKind.RelativeOrAbsolute)),Stretch = Stretch.None} : new SolidColorBrush(Colors.Black);
            panorama.Visibility = !show ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            loginCtrl.Visibility = show ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            //loginCtrl.companyBox.Visibility = !show ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            //loginCtrl.loginBox.Visibility = show ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            loginCtrl.ShowLoginControls(show);
            loginCtrl.ShowApplicationBar();

            if(!show)
            {
                BuildApplicationBar();
                UpdateInfoControl.ShowUpdateInfo();
            }
        }

        public void BuildApplicationBar()
        {
            ApplicationBar = null;
            if (mainPanorama.SelectedIndex <= 0)
            {
                myProfileCtrl.BuildApplicationBar();
            }
        }


        async void login_LoggingChanged(object sender, EventArgs e)
        {
            try
            {
                BugSense.BugSenseHandler.Instance.UserIdentifier=ApplicationState.Current.SessionData.Profile.UserName;
            }
            catch (Exception)
            {
            }

            BuildApplicationBar();
            ShowLogin(false);

            myProfileCtrl.Fill(ApplicationState.Current.ProfileInfo);
            
            await todayCtrl.Fill(DateTime.Now);

            checkCrashState();

            //check if user started appliaction from the secondary live tile and if yes then open calendar
            string entryType;
            if (NavigationContext.QueryString.TryGetValue("EntryType", out entryType))
            {
                todayCtrl.DeepLink(entryType);
                NavigationContext.QueryString.Remove("EntryType");
            }
        }
        
        

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate("/Pages/SettingsPage.xaml");
        }

        private void btnRefreshCalendar_Click(object sender, RoutedEventArgs e)
        {
            //ApplicationState.Current.CurrentTrainingPlan = null;
            
            ApplicationState.Current.ClearTrainingDays();
            //ApplicationState.Current.TrainingDays.Clear();
            todayCtrl.Fill(DateTime.Now);
        }


        private void btnRate_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BuildApplicationBar();
        }

        private void btnBodyInstructor_Click(object sender, RoutedEventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Instructor_InstructorPart, this, AccountType.Instructor))
            {
                return;
            }
            NavigationService.Navigate(new Uri("/Pages/BodyInstructorPage.xaml",UriKind.RelativeOrAbsolute));
        }

        private void btnTutorial_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(Constants.TutorialUrl);
            task.Show();
        }

    }
}