using System;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class ProfileInfoPage : AnimatedBasePage
    {
        private UserViewModel viewModel;

        public ProfileInfoPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        public UserSearchDTO SelectedUser
        {
            get { return ApplicationState.Current.CurrentViewUser; }
            set
            {
                ApplicationState.Current.CurrentViewUser = value;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            updateOfflineModeGui();

            ApplicationState.Current.CurrentBrowsingTrainingDays =new TrainingDaysHolder( SelectedUser);
            viewModel = new UserViewModel(SelectedUser);
            viewModel.OperationCompleted += new EventHandler(viewModel_OperationCompleted);
            DataContext = viewModel;
            sizesCtrl.Fill(viewModel.Wymiary,null);
            awardsCtrl.User = viewModel.User;
            StateHelper stateHelper = new StateHelper(this.State);
            var pivotItem = stateHelper.GetValue<int>("PivotSelectedTab", 0);
            pivot.SelectedIndex = pivotItem;

            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsPopupOpen())
            {
                ApplicationState.Current.CurrentBrowsingTrainingDays = null;
            }
            base.OnBackKeyPress(e);
        }

        private void updateOfflineModeGui()
        {
            pnlLinks.Visibility = !ApplicationState.Current.IsOffline ? Visibility.Visible : Visibility.Collapsed;
            lblOfflineMode.Visibility = ApplicationState.Current.IsOffline ? Visibility.Visible : Visibility.Collapsed;
            lblOfflineMode1.Visibility = ApplicationState.Current.IsOffline ? Visibility.Visible : Visibility.Collapsed;
            lblStatus.Visibility = !ApplicationState.Current.IsOffline ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            var sendMessage = e.Content as SendMessagePage;
            var statPage = e.Content as StatisticsPage;
            if(statPage!=null)
            {
                statPage.User = SelectedUser;
            }
            if (sendMessage != null)
            {
                sendMessage.Receiver = viewModel.User;
            }
            base.OnNavigatedFrom(e);
            this.State["PivotSelectedTab"] = pivot.SelectedIndex;
        }

        void viewModel_OperationCompleted(object sender, EventArgs e)
        {
            sizesCtrl.Fill(viewModel.Wymiary, null);
            lblStatus.Visibility = !viewModel.HasAccessToMeasurements
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Collapsed;
            sizesCtrl.Visibility = viewModel.HasAccessToMeasurements
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Collapsed;

            lblAboutStatus.Text = ApplicationStrings.UserViewModel_About_Empty;
            lblAboutStatus.Visibility = viewModel.HasAbout
                               ? System.Windows.Visibility.Collapsed
                               : System.Windows.Visibility.Visible;
            progressBar.ShowProgress(false);
        }


        private void btnRejectFriendship_Click(object sender, RoutedEventArgs e)
        {
            if(BAMessageBox.Ask(string.Format(ApplicationStrings.ProfileInfoPage_QRejectFriendship,viewModel.UserName))==MessageBoxResult.Cancel)
            {
                return;
            }
            progressBar.ShowProgress(true,ApplicationStrings.ProfileInfoPage_ProgressSend);
            viewModel.RejectFriendship();
        }

        private void btnRemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (BAMessageBox.Ask(string.Format(ApplicationStrings.ProfileInfoPage_QRemoveFromFavorites, viewModel.UserName)) == MessageBoxResult.Cancel)
            {
                return;
            }
            progressBar.ShowProgress(true, ApplicationStrings.ProfileInfoPage_ProgressSend);
            viewModel.RemoveFromFavorites();
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(pivot.SelectedIndex>0 && !ApplicationState.Current.IsOffline)
            {
                if (viewModel.User.ProfileInfo == null)
                {
                    progressBar.ShowProgress(true, ApplicationStrings.ProfileInfoPage_ProgressRetrieveProfileDetails);
                    if (viewModel.HasAccessToMeasurements)
                    {
                        lblStatus.Text = ApplicationStrings.Loading;

                    }
                    lblAboutStatus.Text = ApplicationStrings.Loading;
                    viewModel.LoadDetails();
                }
                else
                {
                    lblAboutStatus.Text = ApplicationStrings.UserViewModel_About_Empty;
                    lblAboutStatus.Visibility = viewModel.HasAbout
                                       ? System.Windows.Visibility.Collapsed
                                       : System.Windows.Visibility.Visible;
                    sizesCtrl.Visibility = viewModel.HasAccessToMeasurements
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Collapsed;
                    lblStatus.Visibility = !viewModel.HasAccessToMeasurements
                                       ? System.Windows.Visibility.Visible
                                       : System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void btnShowCalendar_Click(object sender, RoutedEventArgs e)
        {
            ApplicationState.Current.CurrentBrowsingTrainingDays=new TrainingDaysHolder(SelectedUser);
            this.Navigate("/Pages/CalendarPage.xaml");

        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate("/Pages/SendMessagePage.xaml");
        }

        private void btnAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (BAMessageBox.Ask(string.Format(ApplicationStrings.ProfileInfoPage_QAddToFavorites, viewModel.UserName)) == MessageBoxResult.Cancel)
            {
                return;
            }
            progressBar.ShowProgress(true, ApplicationStrings.ProfileInfoPage_ProgressSend);
            viewModel.AddToFavorites();
        }

        private void btnInviteFriendship_Click(object sender, RoutedEventArgs e)
        {
            if (BAMessageBox.Ask(string.Format(ApplicationStrings.ProfileInfoPage_QInviteFriendship, viewModel.UserName)) == MessageBoxResult.Cancel)
            {
                return;
            }
            progressBar.ShowProgress(true, ApplicationStrings.ProfileInfoPage_ProgressSend);
            viewModel.InviteFriendship();
        }

        private void btnShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate("/Pages/StatisticsPage.xaml");
        }
    }
}