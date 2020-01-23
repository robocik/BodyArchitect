using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls
{
    public enum TrainingDaysAchievements
    {
        Rank0,
        Rank1=30,
        Rank2=60,
        Rank3=120,
        Rank4=240,
        Rank5=480,
        Rank6=1000
    }

    public enum WorkoutPlansAchievements
    {
        Rank0,
        Rank1 = 5,
        Rank2 = 10,
        Rank3 = 20,
        Rank4 = 35,
        Rank5 = 60,
        Rank6 = 100
    }

    public enum VotingsAchievements
    {
        Rank0,
        Rank1=10,
        Rank2=30,
        Rank3=60,
        Rank4=120
    }

    public class AchievementsHelper
    {
        public static string CategorySportName
        {
            get { return EnumLocalizer.Default.Translate(AchievementCategory.Sport); }
        }

        public static string CategoryFamousName
        {
            get { return EnumLocalizer.Default.Translate(AchievementCategory.Famous); }
        }

        public static string CategorySocialName
        {
            get { return EnumLocalizer.Default.Translate(AchievementCategory.Social); }
        }
        static public string GetIconForRank(AchievementRank rank)
        {
            if (rank == AchievementRank.Rank6)
            {
                return @"..\Resources\WorkoutPlansAwards\bullet-black.png";
            }
            if (rank == AchievementRank.Rank5)
            {
                return @"..\Resources\WorkoutPlansAwards\bullet-red.png";
            }
            if (rank == AchievementRank.Rank4)
            {
                return @"..\Resources\WorkoutPlansAwards\bullet-yellow.png";
            }
            if (rank == AchievementRank.Rank3)
            {
                return @"..\Resources\WorkoutPlansAwards\bullet-blue.png";
            }
            if (rank == AchievementRank.Rank2)
            {
                return @"..\Resources\WorkoutPlansAwards\bullet-green.png";
            }
            return @"..\Resources\WorkoutPlansAwards\bullet-grey.png";
        }

        static public string GetWPFIconForRedStar(AchievementStar star)
        {
            if (star == AchievementStar.Gold)
            {
                return @"..\Resources\Stars\redGoldStar16.png";
            }
            if (star == AchievementStar.Silver)
            {
                return @"..\Resources\Stars\redSilverStar16.png";
            }
            if (star == AchievementStar.Bronze)
            {
                return @"..\Resources\Stars\redBrownStar16.png";
            }
            return null;
        }

        static public Image GetIconForRedStar(AchievementStar star)
        {
            if (star == AchievementStar.Gold)
            {
                return Icons.StarRedGold;
            }
            if (star == AchievementStar.Silver)
            {
                return Icons.StarRedSilver;
            }
            if (star == AchievementStar.Bronze)
            {
                return Icons.StarRedBronze;
            }
            return null;
        }

        static public string GetWPFIconForBlueStar(AchievementStar star)
        {
            if (star == AchievementStar.Gold)
            {
                return @"..\Resources\Stars\blueGoldStar16.png";
            }
            if (star == AchievementStar.Silver)
            {
                return @"..\Resources\Stars\blueSilverStar16.png";
            }
            if (star == AchievementStar.Bronze)
            {
                return @"..\Resources\Stars\blueBrownStar16.png";
            }
            return null;
        }

        static public Image GetIconForBlueStar(AchievementStar star)
        {
            if (star == AchievementStar.Gold)
            {
                return Icons.StarBlueGold;
            }
            if (star == AchievementStar.Silver)
            {
                return Icons.StarBlueSilver;
            }
            if (star == AchievementStar.Bronze)
            {
                return Icons.StarBlueBronze;
            }
            return null;
        }

        static public string GetWPFIconForGreenStar(AchievementStar star)
        {
            if (star == AchievementStar.Gold)
            {
                return @"..\Resources\Stars\greenGoldStar16.png";
            }
            if (star == AchievementStar.Silver)
            {
                return @"..\Resources\Stars\greenSilverStar16.png";
            }
            if (star == AchievementStar.Bronze)
            {
                return @"..\Resources\Stars\greenBrownStar16.png";
            }
            return null;
        }

        static public Image GetIconForGreenStar(AchievementStar star)
        {
            if (star == AchievementStar.Gold)
            {
                return Icons.StarGreenGold;
            }
            if (star == AchievementStar.Silver)
            {
                return Icons.StarGreenSilver;
            }
            if (star == AchievementStar.Bronze)
            {
                return Icons.StarGreenBronze;
            }
            return null;
        }

        public static string GetRankToolTip( AchievementRank rank, IDictionary<AchievementRank, int> info)
        {
            if (rank == AchievementRank.Rank1)
            {

               return string.Format(DomainModelStrings.UserStatistics_NoStatus_Description_ToolTip, EnumLocalizer.Default.Translate(AchievementRank.Rank2), info[AchievementRank.Rank2]);
            }
            string nextStatus = string.Empty;
            if (rank != AchievementRank.Rank6)
            {
                var nextRank = GetNextEnumValue(rank);
                nextStatus = string.Format(DomainModelStrings.UserStatistics_NextAvailableStatus_ToolTip, EnumLocalizer.Default.Translate(nextRank), info[nextRank]);
            }

            return string.Format(DomainModelStrings.UserStatistics_Status_Description_ToolTip, EnumLocalizer.Default.Translate(rank), info[rank], nextStatus);
        }

        public static string GetStarToolTip(string category,AchievementStar star)
        {
            return string.Format(ApplicationStrings.UserStatistics_StarToolTip, EnumLocalizer.Default.Translate(star), category);
        }

        static public T GetNextEnumValue<T>(T value)
        {
           return  Enum.GetValues(typeof(T)).Cast<T>().SkipWhile(e => !e.Equals(value)).Skip(1).First();
        }
    }
}
