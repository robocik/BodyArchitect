﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum ContentType
    {
        Text,
        Html,
        Rtf
    }

    public enum BlogCommentOperationType
    {
        Add
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class BlogCommentOperation
    {
        [DataMember]
        public BlogCommentOperationType OperationType { get; set; }

        [DataMember]
        public int BlogEntryId { get; set; }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "BlogCommentOperation_BlogComment_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        public BlogCommentDTO BlogComment { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class BlogCommentDTO:BAObject
    {
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

        [DataMember]
        public ContentType CommentType { get; set; }
    }
}
