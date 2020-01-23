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
    public enum DosageType
    {
        Grams,
        Tablets,
        Units,
        Servings,
        MiliGrams
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class SuplementItemDTO : BAGlobalObject, ICommentable, ICloneable
    {
        public SuplementItemDTO()
        {
            Time = new BATimeDTO();
        }

        [DataMember]
        [NotNullValidator]
        [ObjectValidator]
        public BATimeDTO Time { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.CommentColumnLength, MessageTemplateResourceName = "SuplementItemDTO_Content_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Comment
        {
            get;
            set;
        }

        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "SuplementItemDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [NotNullValidator]
        [ObjectValidator]
        [DataMember]
        public SuplementsEntryDTO SuplementsEntry { get; set; }

        [DataMember]
        public SuplementDTO Suplement { get; set; }

        [DataMember]
        public decimal Dosage { get; set; }

        [DataMember]
        public DosageType DosageType { get; set; }

        public object Clone()
        {
            var newItem = new SuplementItemDTO();
            newItem.Suplement = Suplement;
            newItem.Time.DateTime = Time.DateTime;
            newItem.Time.TimeType = Time.TimeType;
            newItem.Name = Name;
            newItem.Dosage = Dosage;
            newItem.DosageType = DosageType;
            newItem.Comment = Comment;
            return newItem;
        }
    }
}
