using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public class DataInfo : FMGlobalObject
    {
        public virtual Guid ExerciseHash { get; set; }

        public virtual Guid WorkoutPlanHash { get; set; }

        public virtual Guid TrainingDayHash { get; set; }

        public virtual Guid ActivityHash { get; set; }

        public virtual Guid CustomerHash { get; set; }

        public virtual Guid ReminderHash { get; set; }

        public virtual Guid CustomerGroupHash { get; set; }

        public virtual Guid MyPlaceHash { get; set; }

        public virtual Guid ScheduleEntryHash { get; set; }

        public virtual Guid SupplementsCycleDefinitionHash { get; set; }

        public virtual Guid MessageHash { get; set; }

        //MessageHash

        //MyTrainingHash

        //Product (ScheduleEntryReservation)

        //Payment

        //PaymentBasket

        //SupplementCycle
    }
}
