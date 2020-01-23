using System;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrProfileEdit.xaml
    /// </summary>
    public partial class usrProfileEdit
    {
        private ProfileDTO profile;

        private ProfileInformationDTO profileInfo;

        public usrProfileEdit()
        {
            InitializeComponent();
        }

        public void Fill(ProfileDTO profile)
        {
            //UserContext.Current.RefreshUserData();
            profileInfo = UserContext.Current.ProfileInformation.Clone();
            this.profile = profile.StandardClone();
            chkAutomaticUpdateMeasurements.IsChecked = this.profile.Settings.AutomaticUpdateMeasurements;
            usrCreateProfile1.Fill(profile);
            usrWymiaryEditor1.Fill(profileInfo.Wymiary);
            usrProfilePrivacy1.Fill(profile);
            usrProfileNotifications1.Fill(profile);
        }


        public ProfileDTO SaveProfile()
        {
            bool valid = true;
            PictureDTO picture = null;
            Dispatcher.Invoke(new Action(delegate
            {
                usrProfileNotifications1.Save(profile);
                usrProfilePrivacy1.Save(profile);
                if (!usrCreateProfile1.SaveProfile(profile))
                {
                    valid = false;
                    return;
                }
                picture = usrProfilePersonalInfo1.Save(profile);
                profileInfo.Wymiary = usrWymiaryEditor1.SaveWymiary(profileInfo.Wymiary);
                
                profile.Settings.AutomaticUpdateMeasurements = chkAutomaticUpdateMeasurements.IsChecked.Value;
            }));

            if (picture != null && (profile.Picture == null || usrProfilePersonalInfo1.ForceUploadImage || picture.Hash != profile.Picture.Hash))
            {
                var info = ServiceManager.UploadImage(picture);
                picture.PictureId = info.PictureId;
                profile.Picture = info;
                PicturesCache.Instance.AddToCache(picture.ToPictureCacheItem());
            }

            if (valid)
            {
                ProfileUpdateData data = new ProfileUpdateData();
                data.Profile = profile;
                data.Wymiary = profileInfo.Wymiary;
                var res = ServiceManager.UpdateProfile(data);
                return res;
            }
            return null;
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selTab = e.AddedItems[0] as TabItem;
            if (selTab == personalInfoTab)
            {
                ParentWindow.RunAsynchronousOperation(delegate
                {
                    usrProfilePersonalInfo1.Fill(profile);

                }, UIHelper.FindVisualParent<ProfileEditWindow>(this).UpdateProgressIndicator);
            }
        }

        private void chkAutomaticUpdateMeasurements_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            usrWymiaryEditor1.ReadOnly = chkAutomaticUpdateMeasurements.IsChecked.Value;
        }

        //private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        //{
        //    if (tpPersonalInfo == e.Page)
        //    {

        //        ParentWindow.RunAsynchronousOperation(delegate
        //        {
        //            usrProfilePersonalInfo1.Fill(profile);
        //        }, this.GetParentControl<ProfileEditWindow>().UpdateProgressIndicator);
        //    }
        //}

        //private void chkAutomaticUpdateMeasurements_CheckedChanged(object sender, EventArgs e)
        //{
        //    usrWymiaryEditor1.ReadOnly = chkAutomaticUpdateMeasurements.Checked;
        //}
    }

    
}
