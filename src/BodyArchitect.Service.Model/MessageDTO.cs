using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum MessageType
    { 
        Custom, InvitationAccepted, InvitationRejected,
        FriendshipRejected, FriendProfileDeleted, ProfileCreated, TrainingDayAdded, BlogCommentAdded, WorkoutPlanVoted, ExerciseVoted
        , FavoriteProfileDeleted
    }

    public enum MessagePriority
    {
        Normal,
        Low,
        High,
        System
    } 

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class MessageDTO:BAObject
    {
        [DataMember]
        public UserDTO Sender { get; set; }

        [DataMember]
        [NotNullValidator]
        public UserDTO Receiver { get; set; }

        [DataMember]
        public MessageType MessageType { get; set; }

        [DataMember]
        public ContentType ContentType { get; set; }

        [DataMember]
        public MessagePriority Priority { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, 2000, MessageTemplateResourceName = "MessageDTO_Content_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Content { get; set; }

        [DataMember]
        [StringLengthValidator(1, 100, MessageTemplateResourceName = "MessageDTO_Topic_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(MessageTemplateResourceName = "MessageDTO_Topic_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Topic { get; set; }

        /// <summary>
        /// UTC TIME
        /// </summary>
        [DataMember]
        public DateTime CreatedDate { get; set; }
    }
}
