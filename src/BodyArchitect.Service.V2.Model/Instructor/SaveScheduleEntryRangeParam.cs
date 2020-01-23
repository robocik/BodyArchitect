using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BodyArchitect.Service.V2.Model.Localization;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    public enum SaveScheduleEntryRangeCopyMode
    {
        OnlyScheduleEntries,
        All
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [HasSelfValidation]
    public class SaveScheduleEntryRangeParam
    {
        public SaveScheduleEntryRangeParam()
        {
            Entries = new List<ScheduleEntryBaseDTO>();
        }

        [DataMember]
        public SaveScheduleEntryRangeCopyMode Mode { get; set; }

        [DataMember]
        public DateTime StartDay { get; set; }

        [DataMember]
        public DateTime EndDay { get; set; }

        [DataMember]
        [ObjectCollectionValidator]
        public List<ScheduleEntryBaseDTO> Entries { get; private set; }

        [DataMember]
        public DateTime? CopyStart { get; set; }

        [DataMember]
        public DateTime? CopyEnd { get; set; }

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if (StartDay > EndDay)
            {
                results.AddResult(new ValidationResult(ValidationStrings.SaveScheduleEntryRangeParam_EndLowerStart, this, "EndDay", null, null));
            }
            if ((CopyEnd == null && CopyStart != null) || (CopyEnd != null && CopyStart == null))
            {
                results.AddResult(new ValidationResult(ValidationStrings.SaveScheduleEntryRangeParam_CopyParametersIncomplete, this, "CopyStart", null, null));
            }
            if (CopyStart > CopyEnd)
            {
                results.AddResult(new ValidationResult(ValidationStrings.SaveScheduleEntryRangeParam_CopyEndLowerCopyStart, this, "CopyEnd", null, null));
            }
        }
    }
}
