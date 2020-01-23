using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [Serializable]
    [KnownType(typeof(TrainingDayDTO))]
    [DebuggerDisplay("Id = {GlobalId},TrainingDate={TrainingDate}")]
    public class TrainingDayInfoDTO : BAGlobalObject
    {
        private DateTime date;

        public TrainingDayInfoDTO()
        {
            AllowComments = true;
        }

        #region Persistent properties

        [DataMember]
        public DateTime TrainingDate
        {
            get { return date.Date; }
            set { date = value; }
        }

        [DataMember]
        public int CommentsCount { get; private set; }

        [DataMember]
        public bool AllowComments { get; set; }

        [DataMember]
        public DateTime? LastCommentDate { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }

        [DataMember]
        public Guid ProfileId { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "TrainingDayDTO_Comment_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Comment { get; set; }

        
        #endregion
    }
}