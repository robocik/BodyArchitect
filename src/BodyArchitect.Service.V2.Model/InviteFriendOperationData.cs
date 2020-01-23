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
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class InviteFriendOperationData
    {
        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, 500, MessageTemplateResourceName = "FriendInvitationDTO_Message_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Message { get; set; }

        [DataMember]
        [NotNullValidator]
        public UserDTO User { get; set; }

        [DataMember]
        public InviteFriendOperation Operation { get; set; }
    }
}
