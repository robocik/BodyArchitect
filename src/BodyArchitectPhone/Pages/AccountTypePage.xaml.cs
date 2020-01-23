using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitectPhone;
using BugSense;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace BodyArchitect.WP7.Pages
{
    public partial class AccountTypePage
    {
        public AccountTypePage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            tbPremium.Text = EnumLocalizer.Default.Translate(AccountType.PremiumUser).ToUpper();
            tbInstructor.Text = EnumLocalizer.Default.Translate(AccountType.Instructor).ToUpper();
            tbBasic.Text = EnumLocalizer.Default.Translate(AccountType.User).ToUpper();
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        void buildApplicationBar()
        {
            
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            ApplicationBar.IsVisible = false;
            if (pivot.SelectedIndex == 0 && !ApplicationState.Current.IsOffline)
            {
                var button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.refresh.rest.png", UriKind.Relative));
                button1.Click += btnRefresh_Click;
                button1.Text = ApplicationStrings.AppBarButton_Refresh;
                ApplicationBar.Buttons.Add(button1);
                ApplicationBar.IsVisible = true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refreshProfileInfo();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            buildApplicationBar();
            fillCurrentLicence();
        }

        void fillCurrentLicence()
        {
            basicBorder.Background = new SolidColorBrush(Colors.Transparent);
            premiumBorder.Background = new SolidColorBrush(Colors.Transparent);
            instructorBorder.Background = new SolidColorBrush(Colors.Transparent);
            if (ApplicationState.Current.ProfileInfo.Licence.AccountType == AccountType.User)
            {
                basicBorder.Background = (Brush)Application.Current.Resources["CustomAccentBrush"];
            }
            else if (ApplicationState.Current.ProfileInfo.Licence.AccountType == AccountType.PremiumUser)
            {
                premiumBorder.Background = (Brush)Application.Current.Resources["CustomAccentBrush"];
            }
            else if (ApplicationState.Current.ProfileInfo.Licence.AccountType == AccountType.Instructor)
            {
                instructorBorder.Background = (Brush)Application.Current.Resources["CustomAccentBrush"];
            }

            tbPoints.Text = ApplicationState.Current.ProfileInfo.Licence.BAPoints.ToString();
            hlAccountType.Content =EnumLocalizer.Default.Translate(ApplicationState.Current.ProfileInfo.Licence.AccountType);
        }



        private void btnBasicAccountType_Click(object sender, RoutedEventArgs e)
        {
            changeAccountType(AccountType.User);
        }

        private void btnPremiumAccountType_Click(object sender, RoutedEventArgs e)
        {
            changeAccountType(AccountType.PremiumUser);
        }

        private void btnInstructorAccountType_Click(object sender, RoutedEventArgs e)
        {
            changeAccountType(AccountType.Instructor);
        }

        void updateButtons(bool enable)
        {
            btnPremium.IsEnabled = enable;
            btnInstructor.IsEnabled = enable;
            btnBasic.IsEnabled = enable;
        }
        void changeAccountType(AccountType accountType)
        {
            if (ApplicationState.Current.ProfileInfo.Licence.AccountType > accountType)
            {
                int accountDiff = accountType - ApplicationState.Current.ProfileInfo.Licence.AccountType;
                int kara = Math.Abs(accountDiff) * ApplicationState.Current.ProfileInfo.Licence.Payments.Kara;
                if (BAMessageBox.Ask(string.Format(ApplicationStrings.AccountTypePage_QChangeAccountToLower, kara)) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            else
            {
                if (BAMessageBox.Ask(string.Format(ApplicationStrings.AccountTypePage_QChangeAccountToHigher)) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            progressBar.ShowProgress(true, ApplicationStrings.AccountTypePage_ChangingAccountType);
            updateButtons(false);
            var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
            {
                var param = new ProfileOperationParam();
                param.Operation = ProfileOperation.AccountType;
                param.ProfileId = ApplicationState.Current.SessionData.Profile.GlobalId;
                param.AccountType = accountType;

                client1.ProfileOperationAsync(ApplicationState.Current.SessionData.Token, param);
                client1.ProfileOperationCompleted -= operationCompleted;
                client1.ProfileOperationCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, a) =>
                                        {
                                            FaultException<BAServiceException> serviceEx = a.Error as FaultException<BAServiceException>;
                                            if (serviceEx != null && serviceEx.Detail.ErrorCode == ErrorCode.ConsistencyException)
                                            {
                                                updateButtons(true);
                                                BAMessageBox.ShowError(ApplicationStrings.AccountTypePage_ErrNotEnoughPoints);
                                                return;
                                            }
                                            if (a.Error != null)
                                            {
                                                BugSenseHandler.Instance.SendExceptionAsync(a.Error);
                                                updateButtons(true);
                                                progressBar.ShowProgress(false);
                                                
                                                BAMessageBox.ShowError(ApplicationStrings.AccountTypePage_ErrCannotChangeAccountType);
                                                return;
                                            }

                                            
                                            refreshProfileInfo();
                                        };

            if (!m.Run())
            {
                updateButtons(true);
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

        private void refreshProfileInfo()
        {
            progressBar.ShowProgress(true, ApplicationStrings.Login_ProgressRetrieveProfile);
            var profileInfoService = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
                                                                                                     {
                                                                                                         var param = new GetProfileInformationCriteria();
                                                                                                         param.UserId = ApplicationState.Current.SessionData.Profile.GlobalId;

                                                                                                         client1.GetProfileInformationAsync(ApplicationState.Current.SessionData.Token, param);
                                                                                                         client1.GetProfileInformationCompleted -= operationCompleted;
                                                                                                         client1.GetProfileInformationCompleted += operationCompleted;

                                                                                                     });
            profileInfoService.OperationCompleted += (s1, a1) =>
                                                         {
                                                             if(a1.Error!=null)
                                                             {
                                                                 updateButtons(true);
                                                                 progressBar.ShowProgress(false);
                                                                 BAMessageBox.ShowError(ApplicationStrings.MyProfileControl_CantRetrieveProfileInfo_ErrMsg);
                                                                 return;
                                                             }
                                                             ApplicationState.Current.ProfileInfo = a1.Result.Result;
                                                             fillCurrentLicence();
                                                             ((App)App.Current).EnsureAdVisible(null);
                                                             updateButtons(true);
                                                             pivot.SelectedIndex = 0;
                                                             progressBar.ShowProgress(false);
                                                         };
            if (!profileInfoService.Run())
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

        private void btnBuyPoints_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri(Settings.ServerUrl + "V2/Payments.aspx?Token=" +ApplicationState.Current.SessionData.Token.SessionId);
            task.Show();
        }

        private void btnChangeAccountType_Click(object sender, RoutedEventArgs e)
        {
            pivot.SelectedIndex = 1;
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buildApplicationBar();
        }

    }
}