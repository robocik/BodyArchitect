using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public abstract class SupplementCycleEntry : FMGlobalObject
    {
        public virtual SupplementCycleDayRepetitions Repetitions { get; set; }

        public virtual string Comment
        {
            get;
            set;
        }

        //public virtual SupplementCycleDefinitionEntryType Type { get; set; }

        public virtual TimeType TimeType { get; set; }

        public virtual SupplementCycleWeek Week { get; set; }
    }

    [Serializable]
    public class SupplementCycleMeasurement : SupplementCycleEntry
    {
    }
}
