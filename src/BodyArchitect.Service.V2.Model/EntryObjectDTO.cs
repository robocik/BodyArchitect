using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public enum ReportStatus
    {
        ShowInReport,
        SkipInReport
    }

    public enum EntryObjectStatus
    {
        Done,
        Planned,
        System
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
    [KnownType(typeof(GPSTrackerEntryDTO))]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    [Serializable]
    [DebuggerDisplay("Id = {GlobalId},TrainingDate={TrainingDay.TrainingDate}")]
    public class EntryObjectDTO : BAGlobalObject, IRemindable
    {
        #region Persistent properties

        [DataMember]
        [DoNotChecksum]
        [SerializerId]
        public int Version { get; set; }

        [DataMember]
        public TimeSpan? RemindBefore { get; set; }

        [DataMember]
        public EntryObjectStatus Status { get; set; }

        [DataMember]
        public string ApplicationName { get; set;}

        [DataMember]
        public ReportStatus ReportStatus
        {
            get;
            set;
        }

        [DataMember]
        public Guid? ReservationId { get; set; }

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
        public virtual MyTrainingLightDTO MyTraining
        {
            get;
            set;
        }

        //[BelongsTo(Lazy = FetchWhen.OnInvoke)]
        [DataMember]
        [NotNullValidator]
        public TrainingDayInfoDTO TrainingDay
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
