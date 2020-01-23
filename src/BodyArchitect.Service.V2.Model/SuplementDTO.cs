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
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class SuplementDTO : BAGlobalObject, ICommentable, IRatingable, IHasName
    {
        public readonly static SuplementDTO Removed;

        static SuplementDTO()
        {
            Removed = new SuplementDTO();
            Removed.Name = "";
        }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.UrlLength, MessageTemplateResourceName = "SupplementDTO_Url_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Url
        {
            get;
            set;
        }

        //UTC
        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public Guid? ProfileId { get; set; }

        [DataMember]
        [StringLengthValidator(1, Constants.NameColumnLength, MessageTemplateResourceName = "SupplementDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator]
        public string Name { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "SupplementDTO_Comment_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Comment { get; set; }

        [DataMember]
        public float Rating
        {
            get;
            set;
        }

        [DataMember]
        public string UserShortComment { get; set; }

        [DataMember]
        public float? UserRating
        {
            get;
            set;
        }

        [DataMember]
        public bool IsProduct { get; set; }

        [DataMember]
        public bool CanBeIllegal { get; set; }
    }
}
