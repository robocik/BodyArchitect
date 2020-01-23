using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class StatisticsViewModel
    {
        private List<StatisticItemViewModel> items = new List<StatisticItemViewModel>();
        private UserSearchDTO user;

        public StatisticsViewModel(UserSearchDTO user)
        {
            this.user = user;
            addStatistic(ApplicationStrings.UserStatistics_TrainingDaysCountText, user.Statistics.TrainingDaysCount, Achievements.GetTrainingDaysCount(user), Achievements.GetTrainingDaysInfo());
            addStatistic(ApplicationStrings.UserStatistics_WorkoutPlansText, user.Statistics.WorkoutPlansCount, Achievements.GetWorkoutPlansCount(user), Achievements.GetWorkoutPlansInfo());
            addStatistic(ApplicationStrings.UserStatistics_FriendsCountText, user.Statistics.FriendsCount, Achievements.GetFriendsCount(user), Achievements.GetFriendsInfo());
            addStatistic(ApplicationStrings.UserStatistics_FollowersCountText, user.Statistics.FollowersCount, Achievements.GetFollowersCount(user), Achievements.GetFollowersInfo());
            addStatistic(ApplicationStrings.UserStatistics_VotesCountText, user.Statistics.VotingsCount, Achievements.GetVotingsCount(user), Achievements.GetVotingsInfo());
            addStatistic(ApplicationStrings.UserStatistics_BlogCommentsCountText, user.Statistics.TrainingDayCommentsCount, Achievements.GetTrainingDayCommentsCount(user), Achievements.GetBlogCommentsInfo());
            addStatistic(ApplicationStrings.UserStatistics_MyBlogCommentsCount, user.Statistics.MyTrainingDayCommentsCount, Achievements.GetMyTrainingDayCommentsCount(user), Achievements.GetMyBlogCommentsInfo());
            addStatistic(ApplicationStrings.UserStatistics_StrengthTrainingEntriesCount, user.Statistics.StrengthTrainingEntriesCount, Achievements.GetStrengthTrainingEntriesCount(user), Achievements.GetStrengthTrainingEntriesInfo());
            addStatistic(ApplicationStrings.UserStatistics_SizeEntriesCount, user.Statistics.SizeEntriesCount, Achievements.GetSizeEntriesCount(user), Achievements.GetSizeEntriesInfo());
            addStatistic(ApplicationStrings.UserStatistics_SupplementsEntriesCount, user.Statistics.SupplementEntriesCount, Achievements.GetSupplementsEntriesCount(user), Achievements.GetSupplementEntriesInfo());
            addStatistic(ApplicationStrings.UserStatistics_BlogEntriesCount, user.Statistics.BlogEntriesCount, Achievements.GetBlogEntriesCount(user), Achievements.GetBlogEntriesInfo());
            addStatistic(ApplicationStrings.UserStatistics_A6WEntriesCount, user.Statistics.A6WEntriesCount, Achievements.GetA6WEntriesCount(user), Achievements.GetA6WEntriesInfo());
            addStatistic(ApplicationStrings.UserStatistics_A6WFullCyclesCount, user.Statistics.A6WFullCyclesCount, Achievements.GetA6WFullCyclesCount(user), Achievements.GetA6WFullCyclesInfo());
            addStatistic(ApplicationStrings.UserStatistics_SupplementsDefinitionsCount, user.Statistics.SupplementsDefinitionsCount, Achievements.GetSupplementsDefinitionsCount(user), Achievements.GetSupplementsDefinitionsInfo());

            //lvStatistics.Items.Add(string.Format(ApplicationStrings.UserStatistics_LastEntryDateText, user.Statistics.LastEntryDate != null ? user.Statistics.LastEntryDate.Value.ToCalendarDate() : string.Empty));
            //if (user.Statistics.LastEntryDate.HasValue)
            //{
            //    StatisticItemViewModel item = new StatisticItemViewModel();
            //    item.Name = ApplicationStrings.UserStatistics_LastEntryDateText.ToUpper();
            //    item.Value = user.Statistics.LastEntryDate.Value.ToCalendarDate();
            //    Items.Add(item);
            //}
            Items.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

        public string Title
        {
            get { return user.UserName.ToUpper(); }
        }

        public List<StatisticItemViewModel> Items
        {
            get { return items; }
        }

        public static T[] GetEnumValues<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
              from field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
              where field.IsLiteral
              select (T)field.GetValue(null)
            ).ToArray();
        }

        static public T GetNextEnumValue<T>(T value)
        {
            return GetEnumValues<T>().SkipWhile(e => !e.Equals(value)).Skip(1).First();
        }

        void addStatistic(string text, int value, AchievementRank rank, IDictionary<AchievementRank, int> info)
        {
            var item = new StatisticItemViewModel();
            item.Name = text.ToUpper();
            item.Value = value.ToString();
            item.StatusIcon = string.Format("/Images/Ranks/{0}.png", rank.ToString());
            item.StatusDescription = getRankDescription(rank, info);
            Items.Add(item);
        }

        private string getRankDescription(AchievementRank rank, IDictionary<AchievementRank, int> info)
        {
            if (rank == AchievementRank.Rank1)
            {

                return string.Format(ApplicationStrings.UserStatistics_NoStatus_Description_ToolTip, EnumLocalizer.Default.Translate(AchievementRank.Rank2), info[AchievementRank.Rank2]);
            }
            string nextStatus = string.Empty;
            if (rank != AchievementRank.Rank6)
            {
                var nextRank = GetNextEnumValue(rank);
                nextStatus = string.Format(ApplicationStrings.UserStatistics_NextAvailableStatus_ToolTip, EnumLocalizer.Default.Translate(nextRank), info[nextRank]);
            }

            return string.Format(ApplicationStrings.UserStatistics_Status_Description_ToolTip, EnumLocalizer.Default.Translate(rank), info[rank], nextStatus);
        }
    }

    public class StatisticItemViewModel
    {
        public string StatusIcon { get; set; }

        public string StatusDescription { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
