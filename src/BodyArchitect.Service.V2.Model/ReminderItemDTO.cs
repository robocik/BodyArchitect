using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public enum ReminderRepetitions
    {
        Once,
        EveryDay,
        EveryWeek,
        EveryMonth,
        EveryYear
    }

    public enum ReminderType
    {
        Custom,
        Birthday,
        ScheduleEntry,
        EntryObject
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ReminderItemDTO:BAGlobalObject
    {
        public ReminderItemDTO()
        {
            DateTime = DateTime.UtcNow;
        }

        [DataMember]
        public Guid ProfileId { get; set; }

        [Required]
        [DataMember]
        [StringLengthValidator(Constants.NameColumnLength, MessageTemplateResourceName = "ReminderItemDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Name { get; set; }

        [DataMember]
        //UTC
        public DateTime DateTime { get; set; }

        //null - only once, otherwise 1,2,3 (days of week)
        //[DataMember]
        //public string RepeatPattern { get; set; }

        [DataMember]
        public ReminderRepetitions Repetitions { get; set; }

        [DataMember]
        public TimeSpan? RemindBefore { get; set; }

        [DataMember]
        public ReminderType Type { get; set; }

        [DataMember]
        public int Version { get; set; }

        //UTC
        [DataMember]
        public DateTime? LastShown { get; set; }

        [DataMember]
        public string ConnectedObject { get; set; }
    }
}
