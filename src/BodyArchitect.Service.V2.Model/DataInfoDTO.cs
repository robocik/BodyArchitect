using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class DataInfoDTO
    {
        [DataMember]
        public Guid ExerciseHash { get; set; }

        [DataMember]
        public Guid WorkoutPlanHash { get; set; }

        [DataMember]
        public Guid TrainingDayHash { get; set; }

        [DataMember]
        public Guid ActivityHash { get; set; }

        [DataMember]
        public Guid CustomerHash { get; set; }

        [DataMember]
        public Guid ReminderHash { get; set; }

        [DataMember]
        public Guid CustomerGroupHash { get; set; }

        [DataMember]
        public Guid MyPlaceHash { get; set; }

        [DataMember]
        public Guid ScheduleEntryHash { get; set; }

        [DataMember]
        public Guid SupplementsCycleDefinitionHash { get; set; }

        [DataMember]
        public Guid MessageHash { get; set; }
    }
}
