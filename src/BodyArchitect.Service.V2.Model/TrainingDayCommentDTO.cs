using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{

    public enum TrainingDayOperationType
    {
        Add
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class TrainingDayCommentOperationParam
    {
        [DataMember]
        public TrainingDayOperationType OperationType { get; set; }

        [DataMember]
        public Guid TrainingDayId { get; set; }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "BlogCommentOperation_BlogComment_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        public TrainingDayCommentDTO Comment { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class TrainingDayCommentDTO:BAGlobalObject
    {
        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        [NotNullValidator]
        public UserDTO Profile { get; set; }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "BlogCommentDTO_Comment_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(1, Constants.CommentColumnLength, MessageTemplateResourceName = "BlogCommentDTO_Comment_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Comment { get; set; }

        /// <summary>
        /// UTC TIME
        /// </summary>
        [DataMember]
        public DateTime DateTime { get; set; }
    }
}
