using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using NavigationService = BodyArchitect.Client.UI.Controls.NavigationService;

namespace BodyArchitect.Client.UI.UserControls
{
    public partial class usrUserInfo : IUserDetailControl, IWeakEventListener
    {
        private ProfileInformationDTO user;

        public usrUserInfo()
        {
            InitializeComponent();
            txtProfileStatus.MaxLength = Constants.ProfileStatusColumnLength;
            CollectionChangedEventManager.AddListener(MessagesReposidory.Instance, this);
        }

        public UserSearchDTO User
        {
            get { return user != null ? user.User : null; }
        }

        public void ShowProgress(bool show)
        {
            mainGrid.SetVisible(user != null && !show);
            //tableLayoutPanel2.Visible = user != null && !show;
            //tableLayoutPanel1.Visible = show;
            progressIndicator.IsRunning = show;
            //if (show)
            //{
            //    progressIndicator1.Start();
            //}
            //else
            //{
            //    progressIndicator1.Stop();
            //}
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UIHelper.BeginInvoke(fillMessagesStatus,Dispatcher);
            
            return true;
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            
            this.user = user;
            if (user != null)
            {
                mainGrid.Visibility=Visibility.Visible;
                profileListEntry.Fill(user.User);
                NavigationService.SetText(txtAbout, user.AboutInformation);
                
                fillProfileStatus(user);
                
                if (user.User.IsMe())
                {
                    btnInvitationsCountStatus.Content = string.Format(Strings.usrUserInfo_InvitationsCount, user.Invitations.Count);

                    if (UserContext.Current.SessionData.LastLoginDate.HasValue)
                    {
                        lblLastLoggedTime.Text = string.Format(Strings.usrUserInfo_LastLogin, UserContext.Current.SessionData.LastLoginDate.Value.ToLocalTime().ToRelativeDate());
                    }
                    fillMessagesStatus();
                    btnInvitationsCountStatus.SetVisible(user.Invitations.Count > 0);
                    lblLastLoggedTime.SetVisible(user.LastLogin.HasValue);
                    lblProfileNotActivated.SetVisible(!user.IsActivated);
                    btnAccountType.Content = EnumLocalizer.Default.Translate(UserContext.Current.ProfileInformation.Licence.CurrentAccountType);
                    tbPoints.Text = UserContext.Current.ProfileInformation.Licence.BAPoints.ToString();

//                    bool profileConfWizard = UserContext.Current.Settings.GetProfileConfigurationWizardShowed(UserContext.Current.CurrentProfile.GlobalId);
//#if RELEASE
//                    profileConfWizard=true;
//#endif
//                    btnProfileConfigurationWizard.SetVisible(!profileConfWizard);
                }
                grAccountType.SetVisible(User.IsMe());
                grInfo.SetVisible(User.IsMe());
                //grStatus.Collapse(User.IsMe());
                Grid.SetRowSpan(grStatistics,User.IsMe()?1:2);
                lblLastLoggedTime.SetVisible(User.IsMe() && UserContext.Current.SessionData.LastLoginDate.HasValue);
                fillStatistics(User);
                fillAwards(User);
                grAbout.SetVisible(!User.IsMe() && !string.IsNullOrEmpty(user.AboutInformation));
                rowAbout.Collapse(!User.IsMe() && !string.IsNullOrEmpty(user.AboutInformation));
            }
            else
            {
                ClearscrollViewer();
            }
        }

        private void fillProfileStatus(ProfileInformationDTO user)
        {
            if (user.User.IsMe())
            {
                txtProfileStatus.Text = user.User.Statistics.Status.Status;
                txtProfileStatus.Visibility = Visibility.Visible;
                lblProfileStatus.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblProfileStatus.Visibility = Visibility.Visible;
                txtProfileStatus.Visibility = Visibility.Collapsed;
                NavigationService.SetText(lblProfileStatus, user.User.Statistics.Status.Status);
            }
        }

        private void fillMessagesStatus()
        {
            if(user==null)
            {
                return;
            }
            if (MessagesReposidory.Instance.IsLoaded)
            {
                btnMessagesCountStatus.Content = string.Format(Strings.usrUserInfo_MessagesCount, MessagesReposidory.Instance.Items.Count);
            }
            bool profileConfWizard = UserContext.Current.Settings.GetProfileConfigurationWizardShowed(UserContext.Current.CurrentProfile.GlobalId);
            btnMessagesCountStatus.SetVisible(MessagesReposidory.Instance.IsLoaded && MessagesReposidory.Instance.Items.Count > 0);
            lblNoStatus.SetVisible((!MessagesReposidory.Instance.IsLoaded || MessagesReposidory.Instance.Items.Count == 0) && user.Invitations.Count == 0 && profileConfWizard);
        }

        void fillStatistics(UserSearchDTO user)
        {
            List<StatisticItemViewModel> items = new List<StatisticItemViewModel>();
            if (user.Statistics != null)
            {
                addStatistic(items,string.Format(Strings.UserStatistics_TrainingDaysCountText, user.Statistics.TrainingDaysCount), Achievements.GetTrainingDaysCount(user), Achievements.GetTrainingDaysInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_WorkoutPlansText, user.Statistics.WorkoutPlansCount), Achievements.GetWorkoutPlansCount(user), Achievements.GetWorkoutPlansInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_SupplementsDefinitionsText, user.Statistics.WorkoutPlansCount), Achievements.GetSupplementsDefinitionsCount(user), Achievements.GetSupplementsDefinitionsInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_FriendsCountText, user.Statistics.FriendsCount), Achievements.GetFriendsCount(user), Achievements.GetFriendsInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_FollowersCountText, user.Statistics.FollowersCount), Achievements.GetFollowersCount(user), Achievements.GetFollowersInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_VotesCountText, user.Statistics.VotingsCount), Achievements.GetVotingsCount(user), Achievements.GetVotingsInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_BlogCommentsCountText, user.Statistics.TrainingDayCommentsCount), Achievements.GetTrainingDayCommentsCount(user), Achievements.GetBlogCommentsInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_MyBlogCommentsCount, user.Statistics.MyTrainingDayCommentsCount), Achievements.GetMyTrainingDayCommentsCount(user), Achievements.GetMyBlogCommentsInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_StrengthTrainingEntriesCount, user.Statistics.StrengthTrainingEntriesCount), Achievements.GetStrengthTrainingEntriesCount(user), Achievements.GetStrengthTrainingEntriesInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_SizeEntriesCount, user.Statistics.SizeEntriesCount), Achievements.GetSizeEntriesCount(user), Achievements.GetSizeEntriesInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_SupplementsEntriesCount, user.Statistics.SupplementEntriesCount), Achievements.GetSupplementsEntriesCount(user), Achievements.GetSupplementEntriesInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_BlogEntriesCount, user.Statistics.BlogEntriesCount), Achievements.GetBlogEntriesCount(user), Achievements.GetBlogEntriesInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_A6WEntriesCount, user.Statistics.A6WEntriesCount), Achievements.GetA6WEntriesCount(user), Achievements.GetA6WEntriesInfo());
                addStatistic(items, string.Format(Strings.UserStatistics_A6WFullCyclesCount, user.Statistics.A6WFullCyclesCount), Achievements.GetA6WFullCyclesCount(user), Achievements.GetA6WFullCyclesInfo());
                if (user.IsMe() || UserContext.Current.ProfileInformation.Licence.CurrentAccountType == AccountType.Administrator)
                {//show last login date only when we view another user. We user ToCalendar to hide a time of login - we show only the date
                    addStatistic(items, string.Format(Strings.UserStatistics_LastEntryDate, user.Statistics.LastEntryDate != null ? user.Statistics.LastEntryDate.Value.ToCalendarDate() : string.Empty));
                    //lvStatistics.Items.Add(string.Format(Strings.UserStatistics_LastLoginDate, user.Statistics.LastLoginDate != null ? user.Statistics.LastLoginDate.Value.ToCalendarDate() : string.Empty));
                }
                //lvStatistics.Items.Add(string.Format(Strings.UserStatistics_LastEntryDate, user.Statistics.LastEntryDate != null ? user.Statistics.LastEntryDate.Value.ToCalendarDate() : string.Empty));
                //addStatistic(items, string.Format(Strings.UserStatistics_LastEntryDate, user.Statistics.LastEntryDate != null ? user.Statistics.LastEntryDate.Value.ToCalendarDate() : string.Empty));
            }
            lvStatistics.ItemsSource = items;
        }

        private void addStatistic(List<StatisticItemViewModel> items, string text)
        {
            StatisticItemViewModel item = new StatisticItemViewModel(text);
            items.Add(item);
        }

        void addStatistic(List<StatisticItemViewModel> items,string text, AchievementRank rank, IDictionary<AchievementRank, int> info)
        {
            StatisticItemViewModel item = new StatisticItemViewModel(text, AchievementsHelper.GetRankToolTip(rank, info), new Uri(AchievementsHelper.GetIconForRank(rank)));
            items.Add(item);
        }

        void fillAwards(UserSearchDTO user)
        {
            
            var green = Achievements.GetGreenStar(user);
            var red = Achievements.GetRedStar(user);
            var blue = Achievements.GetBlueStar(user);
            var awards = new ObservableCollection<AwardViewModel>();
            awards.Add(new AwardViewModel(AchievementsHelper.GetWPFIconForRedStar(red), AchievementsHelper.GetStarToolTip(AchievementsHelper.CategorySportName, red)));
            awards.Add(new AwardViewModel(AchievementsHelper.GetWPFIconForBlueStar(blue), AchievementsHelper.GetStarToolTip(AchievementsHelper.CategoryFamousName, blue)));
            awards.Add(new AwardViewModel(AchievementsHelper.GetWPFIconForGreenStar(green), AchievementsHelper.GetStarToolTip(AchievementsHelper.CategorySocialName, green)));

            
            lstAwards.ItemsSource = awards;
            grAwards.SetVisible(red != AchievementStar.None || blue != AchievementStar.None || green != AchievementStar.None);
            //var blueStar = AchievementsHelper.GetIconForBlueStar(blue);
            //var greenStar = AchievementsHelper.GetIconForGreenStar(green);
            //if (redStar != null || blueStar != null || greenStar != null)
            //{
            //    picRedStar.Image = redStar;
            //    picBlueStar.Image = blueStar;
            //    picBlueStar.Visible = blueStar != null;
            //    picRedStar.Visible = redStar != null;
            //    picGreenStar.Image = greenStar;
            //    picGreenStar.Visible = greenStar != null;
            //    ControlHelper.AddSuperTip(toolTipController1, picGreenStar, null, AchievementsHelper.GetStarToolTip(string.Format("<b>{0}</b>", AchievementsHelper.CategorySocialName), green));
            //    ControlHelper.AddSuperTip(toolTipController1, picBlueStar, null, AchievementsHelper.GetStarToolTip(string.Format("<b>{0}</b>", AchievementsHelper.CategoryFamousName), blue));
            //    ControlHelper.AddSuperTip(toolTipController1, picRedStar, null, AchievementsHelper.GetStarToolTip(string.Format("<b>{0}</b>", AchievementsHelper.CategorySportName), red));
            //    grAwards.Visible = true;
            //}
            //else
            //{
            //    grAwards.Visible = false;
            //}
            //grAwards.SetVisible(Awards.Count > 0);
        }

        public void ClearscrollViewer()
        {
            //tableLayoutPanel2.Visible = false;
            //tableLayoutPanel1.Visible = false;
            mainGrid.Visibility = Visibility.Collapsed;
        }


        public string Caption
        {
            get { return Strings.usrUserInfo_Caption_UserInformation; }
        }

        public ImageSource SmallImage
        {
            get
            {
                BitmapImage source = "LoginUser.png".ToResourceUrl().ToBitmap();
                return source;
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return true;
        }

        private void btnInvitationsCountStatus_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.FindVisualParent<usrUserInformation>(this).ShowInvitations();
        }

        private void btnMessagesCountStatus_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.FindVisualParent<usrUserInformation>(this).ShowMessages();
        }

        private void txtProfileStatus_LostFocus(object sender, RoutedEventArgs e)
        {
            string newStatus = txtProfileStatus.Text;
            if (string.IsNullOrEmpty(newStatus))
            {//normalize string. instead of "" use null always
                newStatus = null;
            }
            if (User.Statistics.Status.Status != newStatus)
            {//change status only if user type something else in the status textbox
                ServiceCommand command = new ServiceCommand(() =>
                    {
                        var param = new ProfileOperationParam();
                        param.Operation = ProfileOperation.SetStatus;
                        param.ProfileId = User.GlobalId;
                        param.Status=new ProfileStatusDTO();
                        param.Status.Status = newStatus;
                        ServiceManager.ProfileOperation(param);
                        User.Statistics.Status.Status = newStatus;
                    });
                ServicePool.Add(command);
            }
        }

        private void btnChangeAccountType_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Views/ChangeAccountTypeView.xaml"));
        }
        
        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            Helper.OpenUrl(ApplicationSettings.ServerUrl + "V2/Payments.aspx?Token=" + UserContext.Current.Token.SessionId);
        }
    }

    public class AwardViewModel
    {
        public AwardViewModel(string imageUrl, string toolTip)
        {
            Image = !string.IsNullOrEmpty(imageUrl)?new Uri(imageUrl,UriKind.Absolute):null;
            ToolTip = toolTip;
        }

        private Uri _image;
        public Uri Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string ToolTip { get; set; }

    }

    public class StatisticItemViewModel
    {
        public StatisticItemViewModel(string text, string toolTip, Uri image)
        {
            Text = text;
            ToolTip = toolTip;
            Image = image;
        }

        public StatisticItemViewModel(string text)
        {
            this.Text = text;
        }

        public string Text { get; set; }

        public string ToolTip { get; set; }

        public Uri Image { get; set; }
    }

    [Export(typeof(IUserDetailControlBuilder))]
    public class usrUserInfoBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new usrUserInfo();
        }
    }
}
