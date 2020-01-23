using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public enum ScheduleEntryState
    {
        Planned,
        Done,
        Cancelled
    }

    [Serializable]
    public class ScheduleEntryBase : FMGlobalObject, IHasReminder
    {
        public ScheduleEntryBase()
        {
            Reservations = new HashSet<ScheduleEntryReservation>();
        }

        //UTC
        public virtual DateTime StartTime { get; set; }

        //UTC
        public virtual DateTime EndTime { get; set; }

        public virtual MyPlace MyPlace { get; set; }

        public virtual Profile Profile { get; set; }
        
        public virtual int Version { get; set; }
        
        public virtual decimal Price { get; set; }

        public virtual ICollection<ScheduleEntryReservation> Reservations { get; set; }

        public virtual ScheduleEntryState State { get; set; }

        public virtual ReminderItem Reminder { get; set; }

        public virtual bool IsLocked
        {
            get { return State != ScheduleEntryState.Planned || Reservations.Count > 0; }
        }

    }

    [Serializable]
    public class ScheduleEntry : ScheduleEntryBase
    {
        public virtual int MaxPersons { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual CustomerGroup CustomerGroup { get; set; }
    }
}
