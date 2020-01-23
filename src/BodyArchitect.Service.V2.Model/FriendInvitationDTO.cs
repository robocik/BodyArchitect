using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public enum InvitationType
    {
        Invite,
        RejectInvitation,
        RejectFriendship,
        Accept,
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class FriendInvitationDTO
    {
        [DataMember]
        public InvitationType InvitationType { get; set; }

        [DataMember]
        [NotNullValidator]
        public UserDTO Inviter { get; set; }

        [DataMember]
        public DateTime CreatedDateTime { get; set; }

        [DataMember]
        [NotNullValidator]
        public UserDTO Invited { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, 500, MessageTemplateResourceName = "FriendInvitationDTO_Message_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Message { get; set; }
    }
}
