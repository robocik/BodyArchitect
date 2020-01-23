using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public enum SupplementCycleDayRepetitions
    {
        EveryDay,
        OnceAWeek,
        StrengthTrainingDay,
        CardioTrainingDay,
        NonTrainingDay
    }


    public enum DosageUnit
    {
        Absolute,
        ON10KgWight
    }


    [Serializable]
    public class SupplementCycleDosage : SupplementCycleEntry
    {
        public virtual Suplement Supplement { get; set; }

        public virtual DosageType DosageType { get; set; }

        public virtual decimal Dosage { get; set; }

        public virtual DosageUnit DosageUnit { get; set; }
        
        public virtual string Name { get; set; }


        
    }
}
