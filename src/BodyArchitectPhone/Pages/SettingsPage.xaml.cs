using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Controls.Cache;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7
{
    public partial class SettingsPage
    {

        public SettingsPage()
        {
            InitializeComponent();

            //var RefreshFrequencyDaySource = new ListPickerValue[]
            //                                    {
            //                                        new ListPickerValue(){Text = ApplicationStrings.SettingsPage_AutomaticCacheRefresh_EveryLogin,Value=0}, 
            //                                        new ListPickerValue(){Text = ApplicationStrings.SettingsPage_AutomaticCacheRefresh_EveryDay,Value=1}, 
            //                                        new ListPickerValue(){Text = ApplicationStrings.SettingsPage_AutomaticCacheRefresh_Every3Days,Value=3}, 
            //                                        new ListPickerValue(){Text = ApplicationStrings.SettingsPage_AutomaticCacheRefresh_Every1Week,Value=7}, 
            //                                        new ListPickerValue(){Text = ApplicationStrings.SettingsPage_AutomaticCacheRefresh_Every2Weeks,Value=14}, 
            //                                        new ListPickerValue(){Text = ApplicationStrings.SettingsPage_AutomaticCacheRefresh_Every1Month,Value=30}, 
            //                                        new ListPickerValue(){Text = ApplicationStrings.SettingsPage_AutomaticCacheRefresh_Never,Value=-1}, 
            //                                    };
            //int oldRefresh = Settings.RefreshFrequencyDays;
            //lpRefreshFrequencyDay.ItemsSource = RefreshFrequencyDaySource;
            //Settings.RefreshFrequencyDays = oldRefresh;

            if (Settings.ExercisesLanguage == "pl-PL" || Settings.ExercisesLanguage=="pl")
            {
                lpExercisesLanguage.SelectedIndex = 2;
            }
            else if (Settings.ExercisesLanguage == "en-US" || Settings.ExercisesLanguage == "en")
            {
                lpExercisesLanguage.SelectedIndex = 1;
            }
            else
            {
                lpExercisesLanguage.SelectedIndex = 0;
            }
            lpExerciseSort.SelectedIndex = (int)Settings.ExercisesSortBy;
            lpNumberOfMonthToRetrieve.SelectedIndex = Settings.NumberOfMonthToRetrieve-1;
            tsCopyValues.IsChecked = Settings.CopyValuesForNewSet;
            tsTreatSuperSetsAsOne.IsChecked = Settings.TreatSuperSetsAsOne;
            //selectRefreshDays();
            tsSendUsageInfo.IsChecked = Settings.SendUsageData;
            tsSendCrashData.IsChecked = Settings.SendCrashData;
            tsPushNotifications.IsChecked = Settings.LiveTileEnabled;
            lpCopyStrengthEntryMode.SelectedIndex=(int)Settings.CopyStrengthTrainingMode;
            tsStartTimer.IsChecked = Settings.StartTimer;
            tsShowSystemTray.IsChecked = Settings.ShowSystemTray;
            tsRunUnderLockScreen.IsChecked = Settings.RunUnderLockScreen;
            tsLocationServices.IsChecked = Settings.LocationServices;
            tsAutoPause.IsChecked = Settings.AutoPause;
            //lpRefreshFrequencyDay.IsEnabled = true;
            lpCopyStrengthEntryMode.IsEnabled = true;
            
        }


        //void selectRefreshDays()
        //{
        //    for (int index = 0; index < lpRefreshFrequencyDay.Items.Count; index++)
        //    {
        //        var item = (ListPickerValue)lpRefreshFrequencyDay.Items[index];
                
        //        int refreshCount;
        //        if (item.Value == Settings.RefreshFrequencyDays)
        //        {
        //            lpRefreshFrequencyDay.SelectedIndex = index;
        //            return;
        //        }
        //    }
        //}

        private void lpExercisesLanguage_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (lpExercisesLanguage!=null && lpExercisesLanguage.SelectedItem != null)
            {
                var lang = (string) ((ListPickerItem) lpExercisesLanguage.SelectedItem).Tag;
                if (lang != Settings.ExercisesLanguage)
                {
                    Settings.ExercisesLanguage = lang;
                    
                    ApplicationState.Current.Cache.Clear();

                    //clearing tips are done in TipsPage. there we check if the langauge is different than cached tips and
                    //only then we refresh
                    //clearTips();
                    if (Settings.ExercisesLanguage != CultureInfo.CurrentUICulture.Name)
                    {
                        MessageBox.Show(ApplicationStrings.SettingsPage_MsgRestartToApplySettings);
                    }
                }
            }
        }

        private void lpExerciseSort_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (lpExerciseSort != null && lpExerciseSort.SelectedItem != null)
            {
                Settings.ExercisesSortBy = (ExerciseSortBy)lpExerciseSort.SelectedIndex;
            }
        }

        private void tsCopyValues_Checked(object sender, RoutedEventArgs e)
        {
            Settings.CopyValuesForNewSet = tsCopyValues.IsChecked.Value;
        }

        private void tsTreatSuperSetsAsOne_Checked(object sender, RoutedEventArgs e)
        {
            Settings.TreatSuperSetsAsOne = tsTreatSuperSetsAsOne.IsChecked.Value;
        }

        private void btnRemovePictureCache_Click(object sender, RoutedEventArgs e)
        {
            PicturesCache.Instance.RemoveFile();
            ApplicationState.ClearOffline();
            ApplicationState.Current.ClearTrainingDays();
            ApplicationState.Current.Cache = null;
            

            
            clearTips();
#if DEBUG
            Settings.InitialAsk = false;//TODO:Remove this!!!!
#endif
        }

        private static void clearTips()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    if (store.FileExists(Constants.TipsFileName))
                    {
                        store.DeleteFile(Constants.TipsFileName);
                    }
                    Settings.TipsDateTime = null;
                }
                catch (IsolatedStorageException ex)
                {
                }
            }
        }

        private void lpNumberOfMonthToRetrieve_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (lpNumberOfMonthToRetrieve != null && lpNumberOfMonthToRetrieve.SelectedItem != null)
            {
                Settings.NumberOfMonthToRetrieve = lpNumberOfMonthToRetrieve.SelectedIndex+1;
            }
        }


        private void tsPushNotifications_Checked(object sender, RoutedEventArgs e)
        {
            if (Settings.LiveTileEnabled != tsPushNotifications.IsChecked.Value)
            {
                TooglePushNotification();
            }
        }

        public void TooglePushNotification()
        {
            progressBar.ShowProgress(true);
            tsPushNotifications.IsEnabled = false;
            Settings.LiveTileEnabled = tsPushNotifications.IsChecked.Value;
            PushNotificationsHelper helper = new PushNotificationsHelper();
            helper.Completed += delegate
                                    {
                     Dispatcher.BeginInvoke(delegate
                                                {
                                                    tsPushNotifications.IsEnabled = true;
                                                    progressBar.ShowProgress(false);
                                                });
                
            };
            try
            {
                if (Settings.LiveTileEnabled)
                {
                    helper.RegisterDevice(ApplicationState.Current.SessionData.Profile.GlobalId);
                }
                else
                {
                    helper.Unregister();
                }
            }
            catch (Exception)
            {
                tsPushNotifications.IsEnabled = true;
                progressBar.ShowProgress(false);
                BAMessageBox.ShowError(ApplicationStrings.SettingsPage_ErrPushNotifications);
            }
        }

        private void tsSendUsageInfo_Checked(object sender, RoutedEventArgs e)
        {
            Settings.SendUsageData = tsSendUsageInfo.IsChecked.Value;
            Settings.SendCrashData = tsSendCrashData.IsChecked.Value;
        }

        //private void lpRefreshFrequencyDay_Changed(object sender, SelectionChangedEventArgs e)
        //{
        //    if (lpRefreshFrequencyDay != null && lpRefreshFrequencyDay.SelectedItem != null)
        //    {
        //        int refreshTime = 7;
        //        var item = (ListPickerValue)lpRefreshFrequencyDay.SelectedItem;
        //        Settings.RefreshFrequencyDays = item.Value;
        //    }
        //}

        private void lpCopyStrengthEntryMode_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (lpCopyStrengthEntryMode != null && lpCopyStrengthEntryMode.SelectedItem != null)
            {
                Settings.CopyStrengthTrainingMode = (CopyStrengthTrainingMode)lpCopyStrengthEntryMode.SelectedIndex;
            }
        }

        

        private void tsShowSystemTray_Checked(object sender, RoutedEventArgs e)
        {
            Settings.ShowSystemTray=SystemTray.IsVisible = tsShowSystemTray.IsChecked.Value;
        }

        private void tsStartTimer_Checked(object sender, RoutedEventArgs e)
        {
            Settings.StartTimer = tsStartTimer.IsChecked.Value;
        }

        private void tsLocationServices_Checked(object sender, RoutedEventArgs e)
        {
            Settings.LocationServices = tsLocationServices.IsChecked.Value;
        }

        private void tsRunUnderLockScreen_Checked(object sender, RoutedEventArgs e)
        {
            Settings.RunUnderLockScreen = tsRunUnderLockScreen.IsChecked.Value;
        }

        private void tsAutoPause_Checked(object sender, RoutedEventArgs e)
        {
            Settings.AutoPause = tsAutoPause.IsChecked.Value;
        }


    }

    public class ListPickerValue
    {
        public string Text { get; set; }

        public int Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}