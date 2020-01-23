using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
 

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    [KnownType(typeof(SupplementCycleMeasurementDTO))]
    [KnownType(typeof(SupplementCycleDosageDTO))]
    public abstract class SupplementCycleEntryDTO : BAGlobalObject
    {
        [DataMember]
        public SupplementCycleDayRepetitions Repetitions { get; set; }

        [DataMember]
        public string Comment
        {
            get;
            set;
        }

        //public virtual SupplementCycleDefinitionEntryType Type { get; set; }

        [DataMember]
        public TimeType TimeType { get; set; }

    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class SupplementCycleMeasurementDTO : SupplementCycleEntryDTO
    {
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class SupplementCycleDosageDTO : SupplementCycleEntryDTO
    {
        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "SupplementCycleDosageDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Name { get; set; }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "SupplementCycleDosageDTO_Supplement_Required", MessageTemplateResourceType = typeof(ValidationStrings))]
        [SkipCloneable]
        public SuplementDTO Supplement { get; set; }
        
        [DataMember]
        public DosageType DosageType { get; set; }

        [DataMember]
        public decimal Dosage { get; set; }

        [DataMember]
        public DosageUnit DosageUnit { get; set; }

    }
}
