using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ProfileSettingsMapping : ClassMapping<ProfileSettings>
    {
        public ProfileSettingsMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            Property(b => b.AutomaticUpdateMeasurements, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.AllowTrainingDayComments, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.NotificationBlogCommentAdded, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.NotificationFollowersChangedCalendar, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.WeightType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.LengthType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.NotificationVoted, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.NotificationFriendChangedCalendar, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.NotificationSocial, y =>
            {
                y.NotNullable(true);
            });
        }
    }

}
