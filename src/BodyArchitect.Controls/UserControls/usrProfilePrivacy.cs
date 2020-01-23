using System;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrProfilePrivacy : XtraUserControl
    {
        public usrProfilePrivacy()
        {
            InitializeComponent();

            foreach (Privacy item in Enum.GetValues(typeof(Privacy)))
            {
                string val = EnumLocalizer.Default.Translate(item);
                cmbCalendarPrivacy.Properties.Items.Add(val);
                cmbSizesPrivacy.Properties.Items.Add(val);
                cmbFriendsListPrivacy.Properties.Items.Add(val);
                cmbBirthdayDatePrivacy.Properties.Items.Add(val);
            }
            
        }

        public void Fill(ProfileDTO profile)
        {
            cmbCalendarPrivacy.SelectedIndex = (int)profile.Privacy.CalendarView;
            cmbSizesPrivacy.SelectedIndex = (int)profile.Privacy.Sizes;
            cmbFriendsListPrivacy.SelectedIndex = (int)profile.Privacy.Friends;
            cmbBirthdayDatePrivacy.SelectedIndex = (int)profile.Privacy.BirthdayDate;
        }

        public void Save(ProfileDTO profile)
        {
            profile.Privacy.CalendarView = (Privacy)cmbCalendarPrivacy.SelectedIndex;
            profile.Privacy.Sizes = (Privacy)cmbSizesPrivacy.SelectedIndex;
            profile.Privacy.Friends = (Privacy)cmbFriendsListPrivacy.SelectedIndex;
            profile.Privacy.BirthdayDate = (Privacy)cmbBirthdayDatePrivacy.SelectedIndex;
        }
    }
}
