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
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class SuplementDTO : BAObject, ICommentable
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

        [DataMember]
        public Guid SuplementId { get; set; }

        [DataMember]
        public int? ProfileId { get; set; }

        [DataMember]
        [StringLengthValidator(1, Constants.NameColumnLength, MessageTemplateResourceName = "SupplementDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator]
        public string Name { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "SupplementDTO_Comment_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Comment { get; set; }
    }
}
