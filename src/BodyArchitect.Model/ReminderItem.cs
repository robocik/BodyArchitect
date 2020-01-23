using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
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

    [Serializable]
    public class ReminderItem:FMGlobalObject
    {
        public virtual Profile Profile { get; set; }

        public virtual string Name { get; set; }
        //UTC
        public virtual DateTime DateTime { get; set; }

        public virtual ReminderRepetitions Repetitions { get; set; }

        //null - only once, otherwise 1,2,3 (days of week)
        /// <summary>
        /// null - only once (datetime must be set)
        /// D:1,2,3, (days of week - every monday for example)
        /// M: (every month - datetime must be set)
        /// Y: (every year - datetime must be set)
        /// </summary>
        //public virtual string RepeatPattern { get; set; }

        public virtual TimeSpan? RemindBefore { get; set; }

        public virtual ReminderType Type { get; set; }

        public virtual int Version { get; set; }
        //UTC
        public virtual DateTime? LastShown { get; set; }

        public virtual string ConnectedObject { get; set; }
    }
}
