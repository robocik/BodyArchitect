using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.Cache;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Client.WCF;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BugSense;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using BodyArchitect.WP7.Controls;
using Microsoft.Phone.Tasks;
using TombstoneHelper;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.Pages
{
    public partial class CreateProfilePage : AnimatedBasePage, ICountrySelector
    {
        private CreateProfileViewModel viewModel;
        private PushNotificationsHelper pushNotification;
        private bool creating;
        private bool cancel;

        public CreateProfilePage()
        {
            InitializeComponent();
            pushNotification = new PushNotificationsHelper();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        public Country Country
        {
            get; set;
        }

        void updateGui()
        {
            createProfileBox.Visibility = creating ? Visibility.Collapsed : Visibility.Visible;
            this.TitlePanel.Visibility = creating ? Visibility.Collapsed : Visibility.Visible;
            this.companyBox.Visibility = !creating ? Visibility.Collapsed : Visibility.Visible;
            mainGrid.Background.Opacity = creating ? 1 : 0;
            if(creating)
            {
                companyBox.BeginAnimation();
            }
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StateHelper helper = new StateHelper(State);
            if (Country == null)
            {
                Country = helper.GetValue<Country>("Country", null);
            }
            if (viewModel == null)
            {
                
                viewModel = helper.GetValue("ViewModel", new CreateProfileViewModel());
                
            }
            viewModel.Country = Country;
            DataContext = viewModel;
            
            //this.RestoreState();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ExtensionMethods.BindFocusedTextBox();
            base.OnNavigatedFrom(e);
            State["ViewModel"] = viewModel;
            State["Country"] = Country;
            //this.SaveState();
        }

        private void Country_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate("/Pages/CountrySelectorPage.xaml");
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Password) || viewModel.Password != viewModel.ConfirmPassword)
            {
                BAMessageBox.ShowError(ApplicationStrings.CreateProfilePage_ErrWrongPasswords);
                return;
            }
            viewModel.Country = Country;

            if (Settings.InitialAsk)
            {
                createProfile();
                return;
            }

            
            
            var ctrl = new FeatureConfirmationControl();
            ctrl.ShowPopup(messagePrompt =>
                               {
                                   messagePrompt.IsCancelVisible = false;
                                   messagePrompt.Completed += (a, s) =>
                                   {
                                       Settings.InitialAsk = true;
                                       createProfile();
                                   };
                               });


            
        }

        private void createProfile()
        {
            if (viewModel.Country==null)
            {
                BAMessageBox.ShowError(ApplicationStrings.CreateProfilePage_ErrSelectCountry);
                return;
            }
            creating = true;
            updateGui();
            progressBar.ShowProgress(true, ApplicationStrings.CreateProfilePage_ProgressCreateAccount);
            ExtensionMethods.BindFocusedTextBox();
            IsHitTestVisible = false;



            var m = new ServiceManager<CreateProfileCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<CreateProfileCompletedEventArgs> operationCompleted)
            {
                var profile = new ProfileDTO();
                profile.Birthday = viewModel.Birthday;
                profile.Gender = viewModel.IsMale ? Gender.Male : Gender.Female;
                profile.CountryId = viewModel.Country.GeoId;
                profile.UserName = viewModel.Username;
                profile.Email = viewModel.Email;
                profile.Settings = new ProfileSettingsDTO();
                profile.Settings.LengthType = (Service.V2.Model.LengthType)lpLengthType.SelectedIndex;
                profile.Settings.WeightType = (Service.V2.Model.WeightType)lpWeightType.SelectedIndex;
                profile.Privacy = new ProfilePrivacyDTO();
                profile.Password = viewModel.Password.ToSHA1Hash();

                client1.CreateProfileAsync(Settings.GetClientInformation(), profile);
                client1.CreateProfileCompleted -= operationCompleted;
                client1.CreateProfileCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, y) =>
            {
                try
                {
                    if (cancel)
                    {
                        progressBar.ShowProgress(false);
                        IsHitTestVisible = true;
                        creating = false;
                        updateGui();
                        return;
                    }
                    if (y.Error != null)
                    {
                        progressBar.ShowProgress(false);
                        IsHitTestVisible = true;
                        creating = false;
                        updateGui();
                        FaultException<ValidationFault> faultEx =y.Error as FaultException<ValidationFault>;
                        FaultException<BAServiceException> baEx = y.Error as FaultException<BAServiceException>;
                        
                        if (faultEx != null)
                        {

                            BAMessageBox.ShowError(faultEx.Detail.Details[0].Key + ":" +faultEx.Detail.Details[0].Message);
                            return;
                        }
                        if (baEx != null && baEx.Detail.ErrorCode==ErrorCode.UniqueException)
                        {
                            BAMessageBox.ShowWarning(ApplicationStrings.ErrUsernameOrEmailNotUnique);
                            return;
                        }
                        BugSenseHandler.Instance.SendExceptionAsync(y.Error);
                        BAMessageBox.ShowError(ApplicationStrings.CreateProfilePage_ErrCreateAccount);
                        return;
                    }
                    else
                    {
                        if (Settings.LiveTileEnabled && Settings.InitialAsk)
                        {
                            try
                            {
                                pushNotification.RegisterDevice(y.Result.Result.Profile.GlobalId);
                            }
                            catch
                            {
                            }
                        }
                        ApplicationState.Current = new ApplicationState();
                        ApplicationState.Current.SessionData = y.Result.Result;
                        //ApplicationState.Current.MyDays = new TrainingDaysHolder(ApplicationState.Current.SessionData.Profile);
                        ApplicationState.Current.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();

                        ApplicationState.Current.TempUserName = viewModel.Username;
                        ApplicationState.Current.TempPassword = viewModel.Password.ToSHA1Hash();

                        Settings.UserName = viewModel.Username;
                        Settings.Password = viewModel.Password;
                        ApplicationState.Current.SessionData.Token.Language = ApplicationState.CurrentServiceLanguage;

                        ApplicationState.Current.Cache.Messages.Load();

                        getProfileInformation();
                    }
                }
                catch (Exception)
                {
                    progressBar.ShowProgress(false);
                    IsHitTestVisible = true;
                    creating = false;
                    updateGui();
                    BAMessageBox.ShowError(ApplicationStrings.CreateProfilePage_ErrCreateAccount);
                }
            };

            if (!m.Run(true))
            {
                IsHitTestVisible = true;
                creating = false;
                updateGui();
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

        private void getProfileInformation()
        {

            var m = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
            {
                GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();

                progressBar.ShowProgress(true, ApplicationStrings.CreateProfilePage_ProgressRetrieveProfile);
                client1.GetProfileInformationAsync(ApplicationState.Current.SessionData.Token, criteria);
                client1.GetProfileInformationCompleted -= operationCompleted;
                client1.GetProfileInformationCompleted += operationCompleted;

            });
            m.OperationCompleted += (s, a1) =>
            {
                 if (cancel)
                          {
                              progressBar.ShowProgress(false);
                              IsHitTestVisible = true;
                              creating = false;
                              return;
                          }
                          progressBar.ShowProgress(false);
                          if (a1.Error != null)
                          {
                              IsHitTestVisible = true;
                              creating = false;
                              updateGui();
                              ApplicationState.Current.SessionData = null;
                              BAMessageBox.ShowError(ApplicationStrings.CreateProfilePage_ErrRetrieveProfile);
                          }
                          else if (a1.Result != null)
                          {
                              ApplicationState.Current.ProfileInfo = a1.Result.Result;
                          }
                          if (NavigationService.CanGoBack)
                          {
                              NavigationService.GoBack();
                          }
            };

            if (!m.Run())
            {
                IsHitTestVisible = true;
                creating = false;
                updateGui();
                progressBar.ShowProgress(false);
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancel = true;
        }

        private void btnGoToWeb_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = (HyperlinkButton) sender;
            WebBrowserTask task = new WebBrowserTask();
            task.URL =(string) btn.Tag;
            task.Show();
        }

        private async void btnCheckUserNameAvailability_Click(object sender, RoutedEventArgs e)
        {
            btnCheckUsername.IsEnabled = false;
            var result = await BAService.CheckProfileNameAvailabilityAsync(txtUserName.Text);
            if (result)
            {
                BAMessageBox.ShowInfo(ApplicationStrings.CreateProfilePage_InfoUsernameAvailable);
            }
            else
            {
                BAMessageBox.ShowWarning(ApplicationStrings.CreateProfilePage_InfoUsernameNotAvailable);
            }
            btnCheckUsername.IsEnabled = true;
        }

    }
}