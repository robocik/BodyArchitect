using System;


namespace BodyArchitect.Model.Old
{
    public enum ReportStatus
    {
        ShowInReport,
        SkipInReport
    }

    [Serializable]
    public abstract class EntryObject:FMObject
    {
       
        #region Persistent properties

        virtual public ReportStatus ReportStatus
        {
            get;
            set;
        }

        public abstract Guid TypeId { get; }

        virtual public string Name
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

        //public virtual Guid? MyTrainingId { get; set; }

        virtual public TrainingDay TrainingDay
        {
            get; set;
        }


        #endregion 

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
