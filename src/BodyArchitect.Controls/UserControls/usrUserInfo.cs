using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.Reporting;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings;
using BodyArchitect.Shared;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    [Export(typeof(IUserDetailControl))]
    public partial class usrUserInfo : usrPictureBaseControl, IUserDetailControl
    {
        private ProfileInformationDTO user;

        public usrUserInfo()
        {
            InitializeComponent();
            this.tableLayoutPanel2.Visible = false;
        }

        public void ShowProgress(bool show)
        {
            tableLayoutPanel2.Visible =user!=null && !show;
            tableLayoutPanel1.Visible = show;
            if(show)
            {
                progressIndicator1.Start();
            }
            else
            {
                progressIndicator1.Stop();
            }
        }

        public void Fill(ProfileInformationDTO user, bool isActive)
        {
            this.user = user;
            if (user != null)
            {
                usrProfileListEntry1.Fill(user.User);
                txtAbout.Text = user.AboutInformation;
                if(user.User.IsMe())
                {
                    lblInvitationsCountStatus.Text= string.Format(ApplicationStrings.usrUserInfo_InvitationsCount,user.Invitations.Count);
                    lblMessagesCountStatus.Text = string.Format(ApplicationStrings.usrUserInfo_MessagesCount, user.Messages.Count);
                    if (UserContext.SessionData.LastLoginDate.HasValue)
                    {
                        lblLastLoggedTime.Text =string.Format(ApplicationStrings.usrUserInfo_LastLogin,UserContext.SessionData.LastLoginDate.Value.ToRelativeDate());
                    }
                    lblMessagesCountStatus.Visible = user.Messages.Count > 0;
                    lblInvitationsCountStatus.Visible = user.Invitations.Count > 0;
                    lblLastLoggedTime.Visible = user.LastLogin.HasValue;
                    lblProfileNotActivated.Visible = !user.IsActivated;
                    bool profileConfWizard =UserContext.Settings.GetProfileConfigurationWizardShowed(UserContext.CurrentProfile.Id);
                    lblNoStatus.Visible = user.Messages.Count == 0 && user.Invitations.Count == 0 && profileConfWizard;
                    lblProfileConfigurationWizard.Visible = !profileConfWizard;
                }
                grInfo.Visible = User.IsMe();
                tableLayoutPanel2.RowStyles[1] = User.IsMe() ? new RowStyle(SizeType.Absolute, 0F) : new RowStyle(SizeType.Percent, 100F);
                tableLayoutPanel2.RowStyles[3] = !User.IsMe() ? new RowStyle(SizeType.Absolute, 0F) : new RowStyle(SizeType.Percent, 100F);
                tableLayoutPanel2.Visible = User.IsMe();
                flowLayoutPanel1.Visible = User.IsMe();
                fillStatistics(User);
                fillAwards(User);
                grAbout.Visible = !User.IsMe();
                tableLayoutPanel2.Visible = true;
            }
            else
            {
                ClearContent();
            }
            
        }

        void fillAwards(UserSearchDTO user)
        {
            var green = Achievements.GetGreenStar(user);
            var red = Achievements.GetRedStar(user);
            var blue = Achievements.GetBlueStar(user);
            var redStar = AchievementsHelper.GetIconForRedStar(red);
            var blueStar = AchievementsHelper.GetIconForBlueStar(blue);
            var greenStar = AchievementsHelper.GetIconForGreenStar(green);
            if (redStar != null || blueStar != null || greenStar!=null)
            {
                picRedStar.Image = redStar;
                picBlueStar.Image = blueStar;
                picBlueStar.Visible = blueStar != null;
                picRedStar.Visible = redStar != null;
                picGreenStar.Image = greenStar;
                picGreenStar.Visible = greenStar != null;
                ControlHelper.AddSuperTip(toolTipController1, picGreenStar, null, AchievementsHelper.GetStarToolTip(string.Format("<b>{0}</b>",AchievementsHelper.CategorySocialName), green));
                ControlHelper.AddSuperTip(toolTipController1, picBlueStar, null, AchievementsHelper.GetStarToolTip(string.Format("<b>{0}</b>",AchievementsHelper.CategoryFamousName), blue));
                ControlHelper.AddSuperTip(toolTipController1, picRedStar, null, AchievementsHelper.GetStarToolTip(string.Format("<b>{0}</b>",AchievementsHelper.CategorySportName), red));
                grAwards.Visible = true;
            }
            else
            {
                grAwards.Visible = false;
            }
        }

        void fillStatistics(UserSearchDTO user)
        {
            lvStatistics.Items.Clear();
            if (user.Statistics != null)
            {
                addStatistic(string.Format(ApplicationStrings.UserStatistics_TrainingDaysCountText, user.Statistics.TrainingDaysCount), Achievements.GetTrainingDaysCount(user), Achievements.GetTrainingDaysInfo());
                addStatistic(string.Format(ApplicationStrings.UserStatistics_WorkoutPlansText, user.Statistics.WorkoutPlansCount), Achievements.GetWorkoutPlansCount(user), Achievements.GetWorkoutPlansInfo());
                addStatistic(string.Format(ApplicationStrings.UserStatistics_FriendsCountText, user.Statistics.FriendsCount), Achievements.GetFriendsCount(user), Achievements.GetFriendsInfo());
                addStatistic(string.Format(ApplicationStrings.UserStatistics_FollowersCountText, user.Statistics.FollowersCount), Achievements.GetFollowersCount(user), Achievements.GetFollowersInfo());
                addStatistic(string.Format(ApplicationStrings.UserStatistics_VotesCountText, user.Statistics.VotingsCount), Achievements.GetVotingsCount(user), Achievements.GetVotingsInfo());
                addStatistic(string.Format(ApplicationStrings.UserStatistics_BlogCommentsCountText, user.Statistics.BlogCommentsCount), Achievements.GetBlogCommentsCount(user), Achievements.GetBlogCommentsInfo());
                addStatistic(string.Format(ApplicationStrings.UserStatistics_MyBlogCommentsCount, user.Statistics.MyBlogCommentsCount), Achievements.GetMyBlogCommentsCount(user), Achievements.GetMyBlogCommentsInfo());
                addStatistic(string.Format(ApplicationStrings.UserStatistics_StrengthTrainingEntriesCount, user.Statistics.StrengthTrainingEntriesCount), Achievements.GetStrengthTrainingEntriesCount(user), Achievements.GetStrengthTrainingEntriesInfo());
                addStatistic(string.Format(DomainModelStrings.UserStatistics_SizeEntriesCount, user.Statistics.SizeEntriesCount), Achievements.GetSizeEntriesCount(user), Achievements.GetSizeEntriesInfo());
                addStatistic(string.Format(DomainModelStrings.UserStatistics_SupplementsEntriesCount, user.Statistics.SupplementEntriesCount), Achievements.GetSupplementsEntriesCount(user), Achievements.GetSupplementEntriesInfo());
                addStatistic(string.Format(DomainModelStrings.UserStatistics_BlogEntriesCount, user.Statistics.BlogEntriesCount), Achievements.GetBlogEntriesCount(user), Achievements.GetBlogEntriesInfo());
                addStatistic(string.Format(DomainModelStrings.UserStatistics_A6WEntriesCount, user.Statistics.A6WEntriesCount), Achievements.GetA6WEntriesCount(user), Achievements.GetA6WEntriesInfo());
                addStatistic(string.Format(DomainModelStrings.UserStatistics_A6WFullCyclesCount, user.Statistics.A6WFullCyclesCount), Achievements.GetA6WFullCyclesCount(user), Achievements.GetA6WFullCyclesInfo());
                if (!user.IsMe() && UserContext.CurrentProfile.Role==Role.Administrator)
                {//show last login date only when we view another user. We user ToCalendar to hide a time of login - we show only the date
                    lvStatistics.Items.Add(string.Format(DomainModelStrings.UserStatistics_LastLoginDate,user.Statistics.LastLoginDate != null? user.Statistics.LastLoginDate.Value.ToCalendarDate(): string.Empty));
                }
                lvStatistics.Items.Add(string.Format(DomainModelStrings.UserStatistics_LastEntryDate,user.Statistics.LastEntryDate != null? user.Statistics.LastEntryDate.Value.ToCalendarDate(): string.Empty));
            }
        }

        void addStatistic(string text,AchievementRank rank,IDictionary<AchievementRank, int> info)
        {
            var item=lvStatistics.Items.Add(text, rank.ToString());
            item.ToolTipText = AchievementsHelper.GetRankToolTip(rank,info);
        }

        public void ClearContent()
        {
            tableLayoutPanel2.Visible = false;
            tableLayoutPanel1.Visible = false;
        }

        public string Caption
        {
            get { return ApplicationStrings.usrUserInfo_Caption_UserInformation; }
        }

        public Image SmallImage
        {
            get { return Icons.Profile; }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return true;
        }

        public UserSearchDTO User
        {
            get { return user!=null?user.User:null; }
        }

        private void lblInvitationsCountStatus_Click(object sender, EventArgs e)
        {
            this.GetParentControl<usrUserInformation>().ShowInvitations();
        }

        private void lblMessagesCountStatus_Click(object sender, EventArgs e)
        {
            this.GetParentControl<usrUserInformation>().ShowMessages();
        }

        private void lblProfileConfigurationWizard_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ShowProfileConfigurationWizard();
        }


        
    }
}
