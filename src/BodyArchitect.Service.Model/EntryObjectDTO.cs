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
    public enum ReportStatus
    {
        ShowInReport,
        SkipInReport
    }

    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    [Serializable]
    public abstract class SpecificEntryObjectDTO : EntryObjectDTO
    {
        public override bool IsLoaded
        {
            get
            {
                return true;
            }
        }
    }

    [KnownType(typeof(SizeEntryDTO))]
    [KnownType(typeof(StrengthTrainingEntryDTO))]
    [KnownType(typeof(SuplementsEntryDTO))]
    [KnownType(typeof(A6WEntryDTO))]
    [KnownType(typeof(BlogEntryDTO))]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    [Serializable]
    public class EntryObjectDTO:BAObject
    {
        #region Persistent properties

        [DataMember]
        public ReportStatus ReportStatus
        {
            get;
            set;
        }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "EntryObjectDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Name
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

        //[BelongsTo(Cascade=CascadeEnum.SaveUpdate,Lazy=FetchWhen.OnInvoke)]
        [DataMember]
        public virtual MyTrainingDTO MyTraining
        {
            get;
            set;
        }

        //[BelongsTo(Lazy = FetchWhen.OnInvoke)]
        [DataMember]
        [NotNullValidator]
        public TrainingDayDTO TrainingDay
        {
            get;
            set;
        }


        #endregion

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public virtual bool IsLoaded
        {
            get
            {
                return false;
            }
        }

    }
}
