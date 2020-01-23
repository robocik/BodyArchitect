using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class ProfileSettingsMapping : ClassMap<ProfileSettings>
    {
        public ProfileSettingsMapping()
        {
            this.LazyLoad();
            Id(x => x.Id);
            Map(x => x.AutomaticUpdateMeasurements).Not.Nullable();
            Map(x => x.NotificationBlogCommentAdded).Not.Nullable();
            Map(x => x.NotificationExerciseVoted).Not.Nullable();
            Map(x => x.NotificationFriendChangedCalendar).Not.Nullable();
            Map(x => x.NotificationWorkoutPlanVoted).Not.Nullable();
        }
    }

}
