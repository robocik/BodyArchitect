using System;

namespace BodyArchitect.Model
{
    [Flags]
    public enum ProfileNotification
    {
        None = 0,
        Message = 1,
        Email = 2
    }

    public enum WeightType
    {
        Kg,
        Pounds
    }

    public enum LengthType
    {
        Cm,
        Inchs
    }

    public class ProfileSettings:FMGlobalObject
    {
        public ProfileSettings()
        {
            NotificationFriendChangedCalendar = ProfileNotification.Message;
            NotificationBlogCommentAdded = ProfileNotification.Message;
            NotificationVoted = ProfileNotification.Message;
            NotificationSocial = ProfileNotification.Message;
            NotificationFollowersChangedCalendar = ProfileNotification.Message;
            AllowTrainingDayComments = true;
        }

        public virtual bool AutomaticUpdateMeasurements { get; set; }

        public virtual bool AllowTrainingDayComments { get; set; }

        public virtual WeightType WeightType { get; set; }

        public virtual LengthType LengthType { get; set; }

        public virtual ProfileNotification NotificationFriendChangedCalendar { get; set; }

        public virtual ProfileNotification NotificationBlogCommentAdded { get; set; }

        public virtual ProfileNotification NotificationVoted { get; set; }

        //todo:check where this setting is used
        public virtual ProfileNotification NotificationSocial { get; set; }

        public virtual ProfileNotification NotificationFollowersChangedCalendar { get; set; }
    }
}
