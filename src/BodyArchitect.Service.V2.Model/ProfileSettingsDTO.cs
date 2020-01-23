using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    [Flags]
    public enum  ProfileNotification
    {
        None=0,
        Message=1,
        Email=2
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


    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ProfileSettingsDTO:BAGlobalObject
    {
        public ProfileSettingsDTO()
        {
            AllowTrainingDayComments = true;
        }

        [DataMember]
        public bool AllowTrainingDayComments { get; set; }

        [DataMember]
        public bool AutomaticUpdateMeasurements { get; set; }

        [DataMember]
        public WeightType WeightType { get; set; }

        [DataMember]
        public LengthType LengthType { get; set; }

        [DataMember]
        public ProfileNotification NotificationFriendChangedCalendar { get; set; }

        [DataMember]
        public ProfileNotification NotificationBlogCommentAdded { get; set; }

        [DataMember]
        public ProfileNotification NotificationVoted { get; set; }

        [DataMember]
        public ProfileNotification NotificationSocial { get; set; }

        [DataMember]
        public ProfileNotification NotificationFollowersChangedCalendar { get; set; }
    }
}
