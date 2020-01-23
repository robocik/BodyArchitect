using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrProfilePrivacy.xaml
    /// </summary>
    public partial class usrProfilePrivacy 
    {
        public usrProfilePrivacy()
        {
            InitializeComponent();

            foreach (Privacy item in Enum.GetValues(typeof(Privacy)))
            {
                string val = EnumLocalizer.Default.Translate(item);
                cmbCalendarPrivacy.Items.Add(val);
                cmbSizesPrivacy.Items.Add(val);
                cmbFriendsListPrivacy.Items.Add(val);
                cmbBirthdayDatePrivacy.Items.Add(val);
            }
            cmbCalendarPrivacy.IsEnabled = UserContext.IsPremium;
            cmbSizesPrivacy.IsEnabled = UserContext.IsPremium;

            hlpCalendarPrivacy.SetVisible(!UserContext.IsPremium);
            hlpSizePrivacy.SetVisible(!UserContext.IsPremium);
        }

        public void Fill(ProfileDTO profile)
        {
            var calendarPrivacy = UserContext.IsPremium ? profile.Privacy.CalendarView : Privacy.Public;
            var measurementsPrivacy = UserContext.IsPremium ? profile.Privacy.Sizes : Privacy.Public;
            cmbCalendarPrivacy.SelectedIndex = (int)calendarPrivacy;
            cmbSizesPrivacy.SelectedIndex = (int)measurementsPrivacy;
            cmbFriendsListPrivacy.SelectedIndex = (int)profile.Privacy.Friends;
            cmbBirthdayDatePrivacy.SelectedIndex = (int)profile.Privacy.BirthdayDate;
            chkAllowComments.IsChecked = profile.Settings.AllowTrainingDayComments;
        }

        public void Save(ProfileDTO profile)
        {
            profile.Privacy.CalendarView = (Privacy)cmbCalendarPrivacy.SelectedIndex;
            profile.Privacy.Sizes = (Privacy)cmbSizesPrivacy.SelectedIndex;
            profile.Privacy.Friends = (Privacy)cmbFriendsListPrivacy.SelectedIndex;
            profile.Privacy.BirthdayDate = (Privacy)cmbBirthdayDatePrivacy.SelectedIndex;
            profile.Settings.AllowTrainingDayComments = chkAllowComments.IsChecked.Value;
        }

    }
}
