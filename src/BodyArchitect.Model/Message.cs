using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    //public enum MessageType {
    //    Custom,
    //    InvitationAccepted, //NotificationSocial
    //    InvitationRejected, //NotificationSocial
    //    FriendshipRejected,//NotificationSocial
    //    FriendProfileDeleted,//NotificationSocial
    //    ProfileCreated,
    //    TrainingDayAdded,
    //    TrainingDayCommentAdded,
    //    WorkoutPlanVoted,//NotificationVoted
    //    ExerciseVoted,//NotificationVoted
    //    FavoriteProfileDeleted,//NotificationSocial
    //    SupplementCycleDefinitionVoted //NotificationVoted
    //}

    public enum MessagePriority
    {
        Normal,
        Low,
        High,
        System
    } 

    public class Message:FMGlobalObject
    {
        public virtual Profile Sender { get; set; }

        public virtual Profile Receiver { get; set; }

        public virtual string Content { get; set; }

        public virtual string Topic { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual MessagePriority Priority { get; set; }
    }
}
