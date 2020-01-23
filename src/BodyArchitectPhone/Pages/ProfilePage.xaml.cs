using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Client.WCF;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Controls.Cache;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BugSense;
using ImageTools;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TombstoneHelper;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.Pages
{
    public partial class ProfilePage : ICountrySelector
    {
        private ProfileViewModel viewModel;
        private PhotoChooserTask task;
        private BitmapSource picture;
        private Guid pictureId;

        public ProfilePage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            buildApplicationBar();
            task = new PhotoChooserTask();
            task.PixelHeight = 120;
            task.PixelWidth = 120;
            task.ShowCamera = true;
            task.Completed += (a, s) =>
            {
                if (s.TaskResult == TaskResult.OK && s.ChosenPhoto != null)
                {
                    picture = new BitmapImage();
                    picture.SetSource(s.ChosenPhoto);
                }
            };
        }


        void buildApplicationBar()
        {
            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.save.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnSave_Click);
            button1.Text = ApplicationStrings.AppBarButton_Save;
            ApplicationBar.Buttons.Add(button1);

        }

        public Country Country
        {
            get; set;
        }

        public Guid PictureId
        {
            get
            {
                if (pictureId == Guid.Empty)
                {
                    if (ApplicationState.Current.SessionData.Profile.Picture != null &&
                        ApplicationState.Current.SessionData.Profile.Picture.PictureIdk__BackingField != Guid.Empty)
                    {
                        pictureId = ApplicationState.Current.SessionData.Profile.Picture.PictureIdk__BackingField;
                    }
                    pictureId = Guid.NewGuid();
                }
                return pictureId;
            }
        }

        public PictureInfoDTO Photo
        {
            get
            {
                return ApplicationState.Current.EditProfileInfo.User.Picture;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
            this.SaveState();
            State["Country"] = Country;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.RestoreState();  // <- second line

            StateHelper helper = new StateHelper(State);
            if (Country == null)
            {
                Country = helper.GetValue<Country>("Country", null);
            }

            if (picture == null)
            {
                profileImage.Fill(Photo);

            }
            else
            {
                profileImage.Source = picture;
            }
            fillNotifications(ApplicationState.Current.EditProfileInfo.Settings);
            viewModel = new ProfileViewModel(ApplicationState.Current.EditProfileInfo);
            if (Country != null)
            {
                viewModel.Country = Country;
            }
            DataContext = viewModel;
            sizesCtrl.Fill(ApplicationState.Current.EditProfileInfo.Wymiary,null);

        }

        void fillNotifications(ProfileSettingsDTO settings)
        {
            chkNotifyFriendChangedCalendarMessage.IsChecked = ((settings.NotificationFriendChangedCalendar & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyFriendChangedCalendarEMail.IsChecked = ((settings.NotificationFriendChangedCalendar & ProfileNotification.Email) == ProfileNotification.Email);

            chkNotifyFollowerChangedCalendarMessage.IsChecked = ((settings.NotificationFollowersChangedCalendar & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyFollowerChangedCalendarEMail.IsChecked = ((settings.NotificationFollowersChangedCalendar & ProfileNotification.Email) == ProfileNotification.Email);

            chkNotifyPlanVotedMessage.IsChecked = ((settings.NotificationVoted & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyPlanVotedEMail.IsChecked = ((settings.NotificationVoted & ProfileNotification.Email) == ProfileNotification.Email);

            chkNotifyTrainingDayCommentMessage.IsChecked = ((settings.NotificationBlogCommentAdded & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyTrainingDayCommentEMail.IsChecked = ((settings.NotificationBlogCommentAdded & ProfileNotification.Email) == ProfileNotification.Email);

            chkNotifyPlanSocialMessage.IsChecked = ((settings.NotificationSocial & ProfileNotification.Message) == ProfileNotification.Message);
            chkNotifyPlanSocialEMail.IsChecked = ((settings.NotificationSocial & ProfileNotification.Email) == ProfileNotification.Email);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ExtensionMethods.BindFocusedTextBox();
            if((!string.IsNullOrEmpty(viewModel.Password1) || !string.IsNullOrEmpty(viewModel.Password2)) && viewModel.Password1!=viewModel.Password2)
            {
                BAMessageBox.ShowError(ApplicationStrings.ProfilePage_ErrDifferentPassword);
                return;
            }
            progressBar.ShowProgress(true, ApplicationStrings.ProfilePage_ProgressSave);
            

            if (picture != null)
            {
                uploadPictureAndSaveProfile();
            }
            else
            {
                saveProfile(null,null);
            }

        }

        private void uploadPictureAndSaveProfile()
        {
            var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
                 {
                     using (OperationContextScope ocs = new OperationContextScope(client1.InnerChannel))
                     {
                         var ggg = (IBodyArchitectAccessService)client1;
                         PictureDTO dto = new PictureDTO();

                         WriteableBitmap _bitmap = new WriteableBitmap(picture);
                         var extImage = _bitmap.ToImage();
                         var st = (MemoryStream)extImage.ToStream();
                         st.Seek(0, SeekOrigin.Begin);
                         dto.ImageStream = st.ToArray();
                         OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("SessionId", "http://MYBASERVICE.TK/", ApplicationState.Current.SessionData.Token.SessionId));
                         OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("PictureId", "http://MYBASERVICE.TK/", PictureId));
                         OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("Hash", "http://MYBASERVICE.TK/", "test"));
                         ApplicationState.AddCustomHeaders();

                         ggg.BeginUploadImage(dto, delegate(IAsyncResult aRes)
                              {
                                  var proxy = (IBodyArchitectAccessService)aRes.AsyncState;
                                  string hash = null;
                                  using (OperationContextScope o = new OperationContextScope(((BodyArchitectAccessServiceClient)proxy).InnerChannel))
                                  {

                                      try
                                      {
                                          proxy.EndUploadImage(aRes);
                                      }
                                      catch (Exception ex)
                                      {
                                          BugSenseHandler.Instance.SendExceptionAsync(ex);
                                          Dispatcher.BeginInvoke(delegate
                                                                     {
                                                                         progressBar.ShowProgress(false);
                                                                         ApplicationBar.EnableApplicationBar(true);
                                                                         BAMessageBox.ShowError(ApplicationStrings.ErrUploadPhoto);
                                                                     });

                                          return;
                                      }

                                      hash = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("Hash", "http://MYBASERVICE.TK/");

                                  }

                                  saveProfile(hash, _bitmap);
                              }, client1);

                     }


                 });
            progressBar.ShowProgress(true,ApplicationStrings.ProfilePage_ProgressUploadPhoto);
            ApplicationBar.EnableApplicationBar(false);
            if(!m.Run())
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

        ProfileNotification getNotification(bool message, bool email)
        {
            ProfileNotification notification = ProfileNotification.None;
            if (message)
            {
                notification |= ProfileNotification.Message;
            }
            if (email)
            {
                notification |= ProfileNotification.Email;
            }
            return notification;
        }

        private void saveProfile(string hash, WriteableBitmap _bitmap)
        {
            var m = new ServiceManager<UpdateProfileCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<UpdateProfileCompletedEventArgs> operationCompleted)
            {
                ProfileUpdateData data=new ProfileUpdateData();
                var copyProfile = ApplicationState.Current.SessionData.Profile.Copy();
                copyProfile.AboutInformation = viewModel.Profile.AboutInformation;
                copyProfile.Birthday = viewModel.Profile.Birthday.Value;
                copyProfile.Gender = viewModel.Profile.User.Gender;
                copyProfile.CountryId = viewModel.Profile.User.CountryId;
                copyProfile.Settings = viewModel.Profile.Settings;
                if(!string.IsNullOrEmpty(viewModel.Password1))
                {
                    copyProfile.Password = viewModel.Password1.ToSHA1Hash();
                }
                copyProfile.Privacy = viewModel.Profile.User.Privacy;
                copyProfile.Picture = viewModel.Profile.User.Picture;

                
                data.Profile = copyProfile;
                data.Wymiary = viewModel.Profile.Wymiary;

                if (picture != null)
                {
                    if (copyProfile.Picture == null)
                    {
                        copyProfile.Picture = new PictureInfoDTO();
                    }

                    copyProfile.Picture.PictureIdk__BackingField = PictureId;
                    copyProfile.Picture.Hashk__BackingField = hash;
                    copyProfile.Picture.SessionIdk__BackingField = ApplicationState.Current.SessionData.Token.SessionId;
                }
                ApplicationState.Current.EditProfileInfo.User.Picture = copyProfile.Picture;
                data.Profile = copyProfile;
                data.Wymiary = ApplicationState.Current.ProfileInfo.Wymiary;

                client1.UpdateProfileAsync(ApplicationState.Current.SessionData.Token, data);
                client1.UpdateProfileCompleted -= operationCompleted;
                client1.UpdateProfileCompleted += operationCompleted;

            });
            
            m.OperationCompleted += (s, a) =>
            {
                if (a.Error != null)
                {
                    BugSenseHandler.Instance.SendExceptionAsync(a.Error);
                    progressBar.ShowProgress(false);
                    ApplicationBar.EnableApplicationBar(true);
                    BAMessageBox.ShowError(ApplicationStrings.ErrSaveProfile);
                    return;
                }
                else
                {
                    if (_bitmap != null)
                    {
                        PicturesCache.Instance.Remove(PictureId);
                        PictureCacheItem item = new PictureCacheItem(_bitmap, PictureId, hash);
                        PicturesCache.Instance.AddToCache(item);
                        PicturesCache.Instance.Notify(a.Result.Result.Picture);
                    }
                    ApplicationState.Current.ProfileInfo = ApplicationState.Current.EditProfileInfo;
                    ApplicationState.Current.SessionData.Profile = a.Result.Result;

                    if (!string.IsNullOrEmpty(viewModel.Password1))
                    {//store new password
                        Settings.Password = viewModel.Password1;
                        
                    }

                    Dispatcher.BeginInvoke(delegate
                                               {
                                                   if (NavigationService.CanGoBack)
                                                   {
                                                       NavigationService.GoBack();
                                                   }
                                               });
                    
                }
            };
            progressBar.ShowProgress(true, ApplicationStrings.ProfilePage_ProgressSave);
            if(!m.Run())
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

        private void Country_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate("/Pages/CountrySelectorPage.xaml");
        }

        private void btnProfileImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                task.Show();
            }
            catch (Exception)
            {
                
            }
            
        }

        private void Menu_Opened(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = false;
            //ContextMenu menu = (ContextMenu)sender;
            //StrengthTrainingItemViewModel item = (StrengthTrainingItemViewModel)menu.Tag;
            //((MenuItem)menu.Items[1]).Visibility = string.IsNullOrEmpty(item.Item.SuperSetGroup) ? Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = true;
        }

        private void mnuDeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            ApplicationState.Current.EditProfileInfo.User.Picture = null;
            picture = null;
            profileImage.Fill(Photo);
        }

        private void chkNotification_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationState.Current.EditProfileInfo.Settings.NotificationVoted = getNotification(chkNotifyPlanVotedMessage.IsChecked.Value, chkNotifyPlanVotedEMail.IsChecked.Value);
            ApplicationState.Current.EditProfileInfo.Settings.NotificationBlogCommentAdded = getNotification(chkNotifyTrainingDayCommentMessage.IsChecked.Value, chkNotifyTrainingDayCommentEMail.IsChecked.Value);
            ApplicationState.Current.EditProfileInfo.Settings.NotificationFriendChangedCalendar = getNotification(chkNotifyFriendChangedCalendarMessage.IsChecked.Value, chkNotifyFriendChangedCalendarEMail.IsChecked.Value);
            ApplicationState.Current.EditProfileInfo.Settings.NotificationSocial = getNotification(chkNotifyPlanSocialMessage.IsChecked.Value, chkNotifyPlanSocialEMail.IsChecked.Value);
            ApplicationState.Current.EditProfileInfo.Settings.NotificationFollowersChangedCalendar = getNotification(chkNotifyFollowerChangedCalendarMessage.IsChecked.Value, chkNotifyFollowerChangedCalendarEMail.IsChecked.Value);

        }

        private void btnCalendarPrivacyHelp_Click(object sender, RoutedEventArgs e)
        {
            UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_PrivateCalendar, this);
        }

        private void btnMeasurementsPrivacyHelp_Click(object sender, RoutedEventArgs e)
        {
            UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_PrivateCalendar, this);
        }
    }
}