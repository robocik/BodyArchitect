using System;
using System.Collections.Generic;

namespace BodyArchitect.Model
{
    [Serializable]
    public class SupplementCycleWeek:FMGlobalObject
    {
        public SupplementCycleWeek()
        {
            Dosages = new HashSet<SupplementCycleEntry>();
            IsRepetitable = true;
        }

        public virtual string Name { get; set; }

        public virtual int CycleWeekStart { get; set; }

        public virtual int CycleWeekEnd { get; set; }

        public virtual ICollection<SupplementCycleEntry> Dosages { get; protected set; }

        public virtual SupplementCycleDefinition Definition { get; set; }

        public virtual string Comment
        {
            get;
            set;
        }

        public virtual bool IsRepetitable { get; set; }
    }
}
