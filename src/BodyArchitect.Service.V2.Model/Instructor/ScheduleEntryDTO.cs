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
using ValidationResult = Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    public enum ScheduleEntryState
    {
        Planned,
        Done,
        Cancelled
    }

    public interface IRemindable
    {
        TimeSpan? RemindBefore { get; set; }
    }

    [KnownType(typeof(ScheduleEntryDTO))]
    [KnownType(typeof(ScheduleChampionshipDTO))]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    [HasSelfValidation]
    public class ScheduleEntryBaseDTO : BAGlobalObject, IRemindable
    {
        public ScheduleEntryBaseDTO()
        {
            Reservations = new List<ScheduleEntryReservationDTO>();
        }

        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator(typeof(ScheduleEntryReservationDTO))]
        public List<ScheduleEntryReservationDTO> Reservations { get; set; }

        //UTC
        [DataMember]
        public DateTime StartTime { get; set; }

        //UTC
        [DataMember]
        public DateTime EndTime { get; set; }

        

        [DataMember]
        public Guid? MyPlaceId { get; set; }

        

        [DataMember]
        [DoNotChecksum]
        [SerializerId]
        public int Version { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public ScheduleEntryState State { get; set; }

        [DataMember]
        public bool IsLocked { get; set; }

        //[DataMember]
        //[ObjectValidator]
        //public ReminderItemDTO Reminder { get; set; }

        [DataMember]
        public TimeSpan? RemindBefore { get; set; }

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if (StartTime > EndTime)
            {
                results.AddResult(new ValidationResult(ValidationStrings.ScheduleEntryDTO_EndLowerStart, this, "EndTime", null, null));
            }

        }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    [HasSelfValidation]
    public class ScheduleEntryDTO : ScheduleEntryBaseDTO
    {
        [DataMember]
        [NotNullValidator]
        public Guid? ActivityId { get; set; }

        [DataMember]
        public Guid? CustomerGroupId { get; set; }

        [DataMember]
        [RangeAttribute(0, 10000)]
        public int MaxPersons { get; set; }
    }


}
