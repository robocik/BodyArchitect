using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public class ProfileSettings:FMObject
    {
        public ProfileSettings()
        {
            NotificationFriendChangedCalendar = true;
            NotificationBlogCommentAdded = true;
            NotificationWorkoutPlanVoted = true;
            NotificationExerciseVoted = true;
        }

        public virtual bool AutomaticUpdateMeasurements { get; set; }

        public virtual bool NotificationFriendChangedCalendar { get; set; }

        public virtual bool NotificationBlogCommentAdded { get; set; }

        public virtual bool NotificationWorkoutPlanVoted { get; set; }

        public virtual bool NotificationExerciseVoted { get; set; }
    }
}
