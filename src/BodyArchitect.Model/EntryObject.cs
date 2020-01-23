using System;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public enum ReportStatus
    {
        ShowInReport,
        SkipInReport
    }

    public enum EntryObjectStatus
    {
        Done,
        Planned,
        System
    }

    public interface IHasReminder
    {
        ReminderItem Reminder { get; set; }
    }

    [Serializable]
    public abstract class EntryObject : FMGlobalObject, IHasReminder, IVersionable
    {
        virtual public ReportStatus ReportStatus
        {
            get;
            set;
        }

        public virtual int Version { get; set; }

        virtual public EntryObjectStatus Status { get; set; }

        public abstract Guid TypeId { get; }

        virtual public string Name
        {
            get;
            set;
        }

        virtual public ScheduleEntryReservation Reservation
        {
            get;
            set;
        }

        public virtual string Comment
        {
            get;
            set;
        }

        virtual public MyTraining MyTraining
        {
            get;
            set;
        }

        public virtual ReminderItem Reminder { get; set; }

        virtual public LoginData LoginData
        {
            get;
            set;
        }

        //public virtual Guid? MyTrainingId { get; set; }

        virtual public TrainingDay TrainingDay
        {
            get; set;
        }


        public virtual bool IsEmpty
        {
         get { return false; }
        }

        public virtual bool IsLoaded
        {
            get
            {
                return false;
            }
        }

    }
}
