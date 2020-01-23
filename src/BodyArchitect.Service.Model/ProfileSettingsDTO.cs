using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ProfileSettingsDTO:BAObject
    {
        [DataMember]
        public bool AutomaticUpdateMeasurements { get; set; }

        [DataMember]
        public bool NotificationFriendChangedCalendar { get; set; }

        [DataMember]
        public bool NotificationBlogCommentAdded { get; set; }

        [DataMember]
        public bool NotificationWorkoutPlanVoted { get; set; }

        [DataMember]
        public bool NotificationExerciseVoted { get; set; }
    }
}
