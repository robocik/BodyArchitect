using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Serializable]
    public class SupplementCycleWeekDTO : BAGlobalObject
    {
        public SupplementCycleWeekDTO()
        {
            Dosages = new List<SupplementCycleEntryDTO>();
            IsRepetitable = true;
        }

        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "SupplementCycleWeekDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int CycleWeekStart { get; set; }

        [DataMember]
        public int CycleWeekEnd { get; set; }

        [DataMember]
        public bool IsRepetitable
        {
            get;
            set;
        }

        [DataMember]
        public string Comment
        {
            get;
            set;
        }

        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator()]
        public List<SupplementCycleEntryDTO> Dosages { get; set; }

    }
}
