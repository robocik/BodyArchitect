using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class ProfileStatisticsMapping : ClassMap<ProfileStatistics>
    {
        public ProfileStatisticsMapping()
        {
            LazyLoad();
            Id(x => x.Id);
            Map(x => x.FollowersCount).Not.Nullable();
            Map(x => x.LastEntryDate).Nullable();
            Map(x => x.LastLoginDate).Nullable();
            Map(x => x.TrainingDaysCount).Not.Nullable();
            Map(x => x.WorkoutPlansCount).Not.Nullable();
            Map(x => x.BlogCommentsCount).Not.Nullable();
            Map(x => x.VotingsCount).Not.Nullable();
            Map(x => x.FriendsCount).Not.Nullable();

            Map(x => x.StrengthTrainingEntriesCount).Not.Nullable();
            Map(x => x.SizeEntriesCount).Not.Nullable();
            Map(x => x.BlogEntriesCount).Not.Nullable();
            Map(x => x.A6WEntriesCount).Not.Nullable();
            Map(x => x.SupplementEntriesCount).Not.Nullable();
            Map(x => x.A6WFullCyclesCount).Not.Nullable();
            Map(x => x.MyBlogCommentsCount).Not.Nullable();
            Map(x => x.LoginsCount).Not.Nullable();
        }
    }
}
