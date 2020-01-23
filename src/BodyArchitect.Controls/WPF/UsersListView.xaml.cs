using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls.WPF
{
    /// <summary>
    /// Interaction logic for UsersListView.xaml
    /// </summary>
    public partial class UsersListView : UserControl
    {
        public UsersListView()
        {
            InitializeComponent();
        }

        public event EventHandler SelectedUserChanged;

        public UserDTO SelectedUser
        {
            get
            {
                if (lstUsers.SelectedItem != null)
                {
                    return ((UserListItem) lstUsers.SelectedItem).User;
                }
                return null;
            }
        }

        [DefaultValue(false)]
        public bool ShowGroups { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get {return lstUsers.SelectedIndex; }
            set { lstUsers.SelectedIndex=value; }
        }

        public void ClearContent()
        {
            lstUsers.ItemsSource = null;
        }

        [Browsable(false)]
        public ItemCollection Items
        {
            get { return lstUsers.Items; }
        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedUserChanged!=null)
            {
                SelectedUserChanged(this, EventArgs.Empty);
            }
        }

        public void Fill(IEnumerable<UserSearchDTO> items)
        {
            lstUsers.ItemsSource = items.Select(x=>new UserListItem(x));

            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(lstUsers.ItemsSource);
            if (ShowGroups)
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("Group");
                myView.GroupDescriptions.Add(groupDescription);
            }
        }
    }

    public class UserListItem
    {
        public UserSearchDTO User { get; private set; }

        public UserListItem(UserSearchDTO user)
        {
            User = user;
        }

        public string Country
        {
            get { return Service.Model.Country.GetCountry(User.CountryId).DisplayName; }
        }

        
        public string Group
        {
            get
            {
                if(User.IsFriend())
                {
                    return ApplicationStrings.UsersListView_Group_Friends;
                }
                else if(User.IsFavorite())
                {
                    return ApplicationStrings.UsersListView_Group_Favorites;
                }
                return ApplicationStrings.UsersListView_Group_Others;
            }
        }

        public string Gender
        {
            get
            {
                return EnumLocalizer.Default.Translate(User.Gender);
            }
        }

        public string CalendarAllowedToolTip {get { return ApplicationStrings.UserListView_CalendarAllowedToolTip; }}

        public string MeasurementsAllowedToolTip { get { return ApplicationStrings.UserListView_MeasurementsAllowedToolTip; } }

        public bool CalendarAllowed { get { return User.HaveAccess(User.Privacy.CalendarView); } }

        public bool MeasurementsAllowed { get { return User.HaveAccess(User.Privacy.Sizes); } }

        public string LastEntryDate
        {
            get
            {
                if (CalendarAllowed && User.Statistics != null && User.Statistics.LastEntryDate.HasValue)
                {
                    return User.Statistics.LastEntryDate.Value.ToCalendarDate();
                }
                return null;
            }
        }

        public string LastEntryDateToolTip
        {
            get
            {
                if (CalendarAllowed && User.Statistics != null && User.Statistics.LastEntryDate.HasValue)
                {
                    return string.Format(ApplicationStrings.UserStatistics_LastEntryDateToolTip,User.Statistics.LastEntryDate.Value.ToLongDateString());
                }
                return null;
            }
        }

        public string LastLoginDate
        {
            get
            {
                if (User.Statistics != null && User.Statistics.LastLoginDate.HasValue)
                {
                    return User.Statistics.LastLoginDate.Value.ToCalendarDate();
                }
                return null;
            }
        }

        public string LastLoginDateToolTip
        {
            get
            {
                if (User.Statistics != null && User.Statistics.LastLoginDate.HasValue)
                {
                    return string.Format(ApplicationStrings.UserStatistics_LastLogin, User.Statistics.LastLoginDate.Value.ToLongDateString());
                }
                return null;
            }
        }

        public string TrainingDaysCount
        {
            get
            {
                if (User.Statistics != null)
                {
                    return User.Statistics.TrainingDaysCount.ToString();
                }
                return null;
            }
        }

        public string TrainingDaysCountToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return ApplicationStrings.UserStatistics_TrainingDaysCount;
                }
                return null;
            }
        }

        public string TrainingDayStatusImage
        {
            get
            {
                var rank=Achievements.GetTrainingDaysCount(User);
                return AchievementsHelper.GetIconForRank(rank);
            }
        }
        public string TrainingDayStatusToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetRankToolTip(Achievements.GetTrainingDaysCount(User), Achievements.GetTrainingDaysInfo());
                }
                return null;
            }
        }
        
        public string FollowersCount
        {
            get
            {
                if (User.Statistics != null)
                {
                    return ( User.Statistics.FollowersCount).ToString();
                }
                return null;
            }
        }

        public string FollowersCountToolTip
        {
            get { return ApplicationStrings.ApplicationStrings_UserStatistics_FollowersCountToolTip; }
        }

        public string FollowersStatusImage
        {
            get
            {
                var rank = Achievements.GetFollowersCount(User);
                return AchievementsHelper.GetIconForRank(rank);
            }
        }
        public string FollowersStatusToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetRankToolTip(Achievements.GetFollowersCount(User), Achievements.GetFollowersInfo());
                }
                return null;
            }
        }


        public string WorkoutPlansCount
        {
            get
            {
                if (User.Statistics != null)
                {
                    return User.Statistics.WorkoutPlansCount.ToString();
                }
                return null;
            }
        }

        public string WorkoutPlansCountToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return ApplicationStrings.UserStatistics_WorkoutPlansCountToolTip;
                }
                return null;
            }
        }

        public string WorkoutPlansStatusImage
        {
            get
            {
                var rank = Achievements.GetWorkoutPlansCount(User);
                return AchievementsHelper.GetIconForRank(rank);
            }
        }
        public string WorkoutPlansStatusToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetRankToolTip(Achievements.GetWorkoutPlansCount(User), Achievements.GetWorkoutPlansInfo());
                }
                return null;
            }
        }

        public Visibility RedStarVisibiltiy
        {
            get
            {
                if (User.Statistics != null)
                {
                    return Achievements.GetRedStar(User) == AchievementStar.None ? Visibility.Collapsed : Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility BlueStarVisibiltiy
        {
            get
            {
                if (User.Statistics != null)
                {
                    return Achievements.GetBlueStar(User) == AchievementStar.None ? Visibility.Collapsed : Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility GreenStarVisibiltiy
        {
            get
            {
                if (User.Statistics != null)
                {
                    return Achievements.GetGreenStar(User) == AchievementStar.None ? Visibility.Collapsed : Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public string RedStarImage
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetWPFIconForRedStar(Achievements.GetRedStar(User));
                }
                return null;
            }
        }

        public string GreenStarImage
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetWPFIconForGreenStar(Achievements.GetGreenStar(User));
                }
                return null;
            }
        }

        public string BlueStarImage
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetWPFIconForBlueStar(Achievements.GetBlueStar(User));
                }
                return null;
            }
        }

        public string RedStarToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetStarToolTip(AchievementsHelper.CategorySportName, Achievements.GetRedStar(User));
                }
                return null;
            }
        }

        public string GreenStarToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetStarToolTip(AchievementsHelper.CategorySocialName, Achievements.GetGreenStar(User));
                }
                return null;
            }
        }

        public string BlueStarToolTip
        {
            get
            {
                if (User.Statistics != null)
                {
                    return AchievementsHelper.GetStarToolTip(AchievementsHelper.CategoryFamousName, Achievements.GetBlueStar(User));
                }
                return null;
            }
        }

        public Visibility LastLoginVisible
        {
            get { return UserContext.CurrentProfile.Role == Role.Administrator?Visibility.Visible:Visibility.Collapsed; }
        }
    }
}
