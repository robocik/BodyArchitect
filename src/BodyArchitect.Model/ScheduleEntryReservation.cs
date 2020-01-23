using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public class ScheduleEntryReservation : Product
    {
        public ScheduleEntryReservation()
        {
            EntryObjects = new HashSet<EntryObject>();
        }

        public virtual ScheduleEntryBase ScheduleEntry { get; set; }

        public virtual DateTime? EnterDateTime { get; set; }

        public virtual DateTime? LeaveDateTime { get; set; }

        public virtual ICollection<EntryObject> EntryObjects { get; set; }

        public virtual DateTime StartTime
        {
            get
            {
                if (ScheduleEntry != null)
                {
                    return ScheduleEntry.StartTime;
                }
                return EnterDateTime.Value;
            }
        }
    }
}
