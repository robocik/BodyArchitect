using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class CommentEntryDTO:BAObject
    {
        [DataMember]
        [NotNullValidator]
        public UserDTO User { get; set; }

        [DataMember]
        [RangeValidator(0, RangeBoundaryType.Inclusive, Constants.VoteStarsNumber, RangeBoundaryType.Inclusive)]
        public float Rating { get; set; }

        [DataMember]
        [StringLengthValidator(1, Constants.ShortCommentColumnLength, MessageTemplateResourceName = "CommentEntryDTO_ShortComment_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string ShortComment { get; set; }

        /// <summary>
        /// UTC TIME
        /// </summary>
        [DataMember]
        public DateTime VotedDate { get; set; }
    }
}
