using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ProfileStatusMapping : ComponentMapping<ProfileStatus>
    {
        public ProfileStatusMapping()
        {
            Property(b => b.Type, y =>
            {
                y.NotNullable(true);

            });
            Property(b => b.Status, y => y.Length(Constants.ProfileStatusColumnLength));
        }
    }

    public class ProfileStatisticsMapping : ClassMapping<ProfileStatistics>
    {
        public ProfileStatisticsMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            Property(b => b.FollowersCount, y => y.NotNullable(true));
            //TODO:Check UTC
            Property(b => b.LastEntryDate, y => y.NotNullable(false));
            Property(b => b.LastLoginDate, y =>
                                               {
                                                   y.NotNullable(false);
                                                   
                                               });
            Property(b => b.TrainingDaysCount, y => y.NotNullable(true));
            Property(b => b.WorkoutPlansCount, y => y.NotNullable(true));
            Property(b => b.TrainingDayCommentsCount, y => y.NotNullable(true));
            Property(b => b.VotingsCount, y => y.NotNullable(true));
            Property(b => b.FriendsCount, y => y.NotNullable(true));

            Property(b => b.StrengthTrainingEntriesCount, y => y.NotNullable(true));
            Property(b => b.SizeEntriesCount, y => y.NotNullable(true));
            Property(b => b.BlogEntriesCount, y => y.NotNullable(true));
            Property(b => b.A6WEntriesCount, y => y.NotNullable(true));
            Property(b => b.SupplementEntriesCount, y => y.NotNullable(true));
            Property(b => b.A6WFullCyclesCount, y => y.NotNullable(true));
            Property(b => b.MyTrainingDayCommentsCount, y => y.NotNullable(true));
            Property(b => b.LoginsCount, y => y.NotNullable(true));
            Property(b => b.SupplementsDefinitionsCount, y => y.NotNullable(true));
            Component(x=>x.Status);
        }
    }
}
