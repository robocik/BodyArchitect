using System;
using System.Collections.Generic;


namespace BodyArchitect.Service.V2.Model
{
    public enum AchievementRank
    {
        Rank1=0,
        Rank2=1,
        Rank3=3,
        Rank4=6,
        Rank5=12,
        Rank6=24
    }

    public enum AchievementStar
    {
        None,
        Bronze,
        Silver,
        Gold
    }

    public enum AchievementCategory
    {
        Sport,
        Famous,
        Social
    }

    public static class Achievements
    {
        static public IDictionary<AchievementRank,int> GetTrainingDaysInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,1000},
                {AchievementRank.Rank5,600},
                {AchievementRank.Rank4,300},
                {AchievementRank.Rank3,150},
                {AchievementRank.Rank2,50},
            };
        }
        
        static public IDictionary<AchievementRank, int> GetFollowersInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,100},
                {AchievementRank.Rank5,40},
                {AchievementRank.Rank4,20},
                {AchievementRank.Rank3,6},
                {AchievementRank.Rank2,3},
            };
        }

        static public IDictionary<AchievementRank, int> GetSupplementsDefinitionsInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,40},
                {AchievementRank.Rank5,20},
                {AchievementRank.Rank4,10},
                {AchievementRank.Rank3,5},
                {AchievementRank.Rank2,1},
            };
        }

        static public IDictionary<AchievementRank, int> GetWorkoutPlansInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,40},
                {AchievementRank.Rank5,20},
                {AchievementRank.Rank4,10},
                {AchievementRank.Rank3,5},
                {AchievementRank.Rank2,1},
            };
        }

        static public IDictionary<AchievementRank, int> GetBlogCommentsInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,400},
                {AchievementRank.Rank5,250},
                {AchievementRank.Rank4,100},
                {AchievementRank.Rank3,50},
                {AchievementRank.Rank2,20},
            };
        }

        static public IDictionary<AchievementRank, int> GetMyBlogCommentsInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,800},
                {AchievementRank.Rank5,400},
                {AchievementRank.Rank4,200},
                {AchievementRank.Rank3,100},
                {AchievementRank.Rank2,50},
            };
        }

        static public IDictionary<AchievementRank, int> GetVotingsInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,200},
                {AchievementRank.Rank5,100},
                {AchievementRank.Rank4,50},
                {AchievementRank.Rank3,25},
                {AchievementRank.Rank2,10},
            };
        }

        static public IDictionary<AchievementRank, int> GetFriendsInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,100},
                {AchievementRank.Rank5,40},
                {AchievementRank.Rank4,20},
                {AchievementRank.Rank3,8},
                {AchievementRank.Rank2,4},
            };
        }

        static public IDictionary<AchievementRank, int> GetStrengthTrainingEntriesInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,1000},
                {AchievementRank.Rank5,400},
                {AchievementRank.Rank4,200},
                {AchievementRank.Rank3,100},
                {AchievementRank.Rank2,30},
            };
        }

        static public IDictionary<AchievementRank, int> GetBlogEntriesInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,1000},
                {AchievementRank.Rank5,400},
                {AchievementRank.Rank4,200},
                {AchievementRank.Rank3,100},
                {AchievementRank.Rank2,50},
            };
        }

        static public IDictionary<AchievementRank, int> GetSupplementEntriesInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,300},
                {AchievementRank.Rank5,150},
                {AchievementRank.Rank4,100},
                {AchievementRank.Rank3,60},
                {AchievementRank.Rank2,30},
            };
        }

        static public IDictionary<AchievementRank, int> GetSizeEntriesInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,300},
                {AchievementRank.Rank5,150},
                {AchievementRank.Rank4,100},
                {AchievementRank.Rank3,60},
                {AchievementRank.Rank2,30},
            };
        }

        static public IDictionary<AchievementRank, int> GetA6WEntriesInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,165},
                {AchievementRank.Rank5,125},
                {AchievementRank.Rank4,85},
                {AchievementRank.Rank3,40},
                {AchievementRank.Rank2,10},
            };
        }

        static public IDictionary<AchievementRank, int> GetA6WFullCyclesInfo()
        {
            return new Dictionary<AchievementRank, int>()
            {
                {AchievementRank.Rank6,5},
                {AchievementRank.Rank5,4},
                {AchievementRank.Rank4,3},
                {AchievementRank.Rank3,2},
                {AchievementRank.Rank2,1},
            };
        }

        static public AchievementRank GetA6WFullCyclesCount(UserSearchDTO user)
        {
            return GetRank(GetA6WFullCyclesInfo(), user.Statistics.A6WFullCyclesCount);
        }

        static public AchievementRank GetA6WEntriesCount(UserSearchDTO user)
        {
            return GetRank(GetA6WEntriesInfo(), user.Statistics.A6WEntriesCount);
        }

        static public AchievementRank GetSizeEntriesCount(UserSearchDTO user)
        {
            return GetRank(GetSizeEntriesInfo(), user.Statistics.SizeEntriesCount);
        }

        static public AchievementRank GetSupplementsEntriesCount(UserSearchDTO user)
        {
            return GetRank(GetSupplementEntriesInfo(), user.Statistics.SupplementEntriesCount);
        }

        static public AchievementRank GetBlogEntriesCount(UserSearchDTO user)
        {
            return GetRank(GetBlogEntriesInfo(), user.Statistics.BlogEntriesCount);
        }

        static public AchievementRank GetStrengthTrainingEntriesCount(UserSearchDTO user)
        {
            return GetRank(GetStrengthTrainingEntriesInfo(), user.Statistics.StrengthTrainingEntriesCount);
        }

        static public AchievementRank GetFriendsCount(UserSearchDTO user)
        {
            return GetRank(GetFriendsInfo(), user.Statistics.FriendsCount);
        }

        static public AchievementRank GetTrainingDaysCount(UserSearchDTO user)
        {
            return GetRank(GetTrainingDaysInfo(), user.Statistics.TrainingDaysCount);
        }

        static public AchievementRank GetFollowersCount(UserSearchDTO user)
        {
            return GetRank(GetFollowersInfo(), user.Statistics.FollowersCount);
        }

        static public AchievementRank GetSupplementsDefinitionsCount(UserSearchDTO user)
        {
            return GetRank(GetSupplementsDefinitionsInfo(), user.Statistics.SupplementsDefinitionsCount);
        }

        static public AchievementRank GetWorkoutPlansCount(UserSearchDTO user)
        {
            return GetRank(GetWorkoutPlansInfo(), user.Statistics.WorkoutPlansCount);
        }

        static public AchievementRank GetTrainingDayCommentsCount(UserSearchDTO user)
        {
            return GetRank(GetBlogCommentsInfo(), user.Statistics.TrainingDayCommentsCount);
        }

        static public AchievementRank GetMyTrainingDayCommentsCount(UserSearchDTO user)
        {
            return GetRank(GetMyBlogCommentsInfo(), user.Statistics.MyTrainingDayCommentsCount);
        }

        static public AchievementRank GetVotingsCount(UserSearchDTO user)
        {
            return GetRank(GetVotingsInfo(), user.Statistics.VotingsCount);
        }

        static private AchievementRank GetRank(IDictionary<AchievementRank, int> rankInfo, int value)
        {
            foreach (var info in rankInfo)
            {
                if (value>=info.Value)
                {
                    return info.Key;
                }
            }
            return AchievementRank.Rank1;
        }


        #region Stars

        public static AchievementStar GetRedStar(UserSearchDTO user)
        {
            var rankTrainingDays=GetTrainingDaysCount(user);
            var rankStrenghtEntry = GetStrengthTrainingEntriesCount(user);
            var rankSizeEntry = GetSizeEntriesCount(user);
            var rankSupplementEntry = GetSupplementsEntriesCount(user);
            var rankA6WCycles = GetA6WFullCyclesCount(user);

            int sum = (int)rankA6WCycles + (int)rankSupplementEntry + (int)rankTrainingDays + (int)rankSizeEntry + (int)rankStrenghtEntry;
            return getStar(sum);
        }

        public static AchievementStar GetBlueStar(UserSearchDTO user)
        {
            var rankFollowers = GetFollowersCount(user);
            var rankFriends = GetFriendsCount(user);
            var rankMyBlogComments = GetMyTrainingDayCommentsCount(user);
            var rankBlogEntries = GetBlogEntriesCount(user);

            int sum = (int)rankFollowers + (int)rankFriends + (int)rankMyBlogComments + (int)rankBlogEntries;
            return getStar(sum);
        }

        //private static AchievementStar getStar(int sum)
        //{
        //    if (sum >= 300)
        //    {
        //        return AchievementStar.Gold;
        //    }
        //    if (sum >= 150)
        //    {
        //        return AchievementStar.Silver;
        //    }
        //    if (sum >= 20)
        //    {
        //        return AchievementStar.Bronze;
        //    }
        //    return AchievementStar.None;
        //}
        private static AchievementStar getStar(int sum)
        {
            if (sum >= 72)
            {
                return AchievementStar.Gold;
            }
            if (sum >= 36)
            {
                return AchievementStar.Silver;
            }
            if (sum >= 9)
            {
                return AchievementStar.Bronze;
            }
            return AchievementStar.None;
        }

        public static AchievementStar GetGreenStar(UserSearchDTO user)
        {
            var rankVotings = GetVotingsCount(user);
            var rankWorkoutPlans = GetWorkoutPlansCount(user);
            var rankSupplementsDefinitions = GetSupplementsDefinitionsCount(user);
            var rankBlogComments = GetTrainingDayCommentsCount(user);

            int sum = (int)rankVotings + 3*(int)rankWorkoutPlans + (int)rankBlogComments + 3*(int)rankSupplementsDefinitions;
            return getStar(sum);
        }
        #endregion

        
    }
}
