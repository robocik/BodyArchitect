using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public enum MessageType {
        Custom, InvitationAccepted, InvitationRejected, FriendshipRejected,
        FriendProfileDeleted,
        ProfileCreated,
        TrainingDayAdded,
        BlogCommentAdded,
        WorkoutPlanVoted,ExerciseVoted,
        FavoriteProfileDeleted
    }

    public enum MessagePriority
    {
        Normal,
        Low,
        Hight,
        System
    } 

    public class Message:FMObject
    {
        public Profile Sender { get; set; }

        public Profile Receiver { get; set; }

        public MessageType MessageType { get; set; }

        public string Content { get; set; }

        public string Topic { get; set; }

        public DateTime CreatedDate { get; set; }

        public MessagePriority Priority { get; set; }

        public ContentType ContentType { get; set; }


    }
}
