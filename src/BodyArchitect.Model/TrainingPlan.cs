using System;
using System.Collections.Generic;
using System.Linq;
using BodyArchitect.Shared;


namespace BodyArchitect.Model
{
    public enum TrainingPlanDifficult
    {
        NotSet,
        Beginner,
        Advanced,
        Professional
    }

    public enum WorkoutPlanPurpose
    {
        NotSet,
        Strength,
        Mass,
        FatLost,
        Definition,
        Other
    }
    public enum TrainingType
    {
        Split,
        FBW,
        HIT,
        ABW,
        HST,
        PushPull,
        ACT,
        Other
    }

    public enum PublishStatus
    {
        Private,
        Published
    }

    public interface IGlobalObject
    {
        Guid GlobalId
        {
            get;
            set;
        }
    }

    public interface ISortable : IRatingable
    {
        string Name { get; }

        DateTime CreationDate { get; set; }
    }

    public interface IRatingable : IGlobalObject
    {
        float Rating { get; set; }

        Profile Profile { get; set; }
    }

    [Serializable]
    public class TrainingPlanSerie:FMGlobalObject
    {
        public virtual int? RepetitionNumberMin
        {
            get; set;
        }

        public virtual int? RepetitionNumberMax
        {
            get; set;
        }

        public virtual string Comment
        {
            get;
            set;
        }

        public virtual bool IsRestPause { get; set; }

        public virtual bool IsSuperSlow { get; set; }

        public virtual SetType RepetitionsType
        {
            get; set;
        }

        public virtual DropSetType DropSet
        {
            get; set;
        }

        public virtual TrainingPlanEntry Entry { get; set; }

        public virtual int Position { get; set; }
    }
    [Serializable]
    public class TrainingPlanEntry:FMGlobalObject
    {
        public TrainingPlanEntry()
        {
            Sets = new HashSet<TrainingPlanSerie>();
        }

        public virtual ExerciseDoneWay DoneWay
        {
            get;
            set;
        }

        public virtual ICollection<TrainingPlanSerie> Sets { get; set; }

        public virtual int? RestSeconds { get; set; }

        public virtual TrainingPlanDay Day { get; set; }

        public virtual string GroupName { get; set; }

        public virtual Exercise Exercise
        {
            get; set;
        }

        public virtual int Position { get; set; }

        public virtual string Comment
        {
            get;
            set;
        }
    }
    [Serializable]
    public class TrainingPlanDay:FMGlobalObject
    {
        public virtual ICollection<TrainingPlanEntry> Entries { get; set; }


        public virtual int Position { get; set; }

        public TrainingPlanDay()
        {
            Entries = new HashSet<TrainingPlanEntry>();
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual TrainingPlan TrainingPlan
        {
            get; set; }

    }

    [Serializable]
    public class TrainingPlan : FMGlobalObject, ISortable
    {
        public TrainingPlan()
        {
            Days = new HashSet<TrainingPlanDay>();
        }

        #region Persistent properties

        public virtual PublishStatus Status { get; set; }
        
        public virtual float Rating
        {
            get;
            set;
        }

        public virtual string Comment
        {
            get;
            set;
        }

        public virtual object Tag { get; set; }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual int RestSeconds
        {
            get;
            set;
        }

        public virtual int Version { get; set; }

        public virtual string Author { get; set; }

        public virtual Profile Profile { get; set; }
     
        public virtual TrainingPlanDifficult Difficult { get; set; }

        public virtual WorkoutPlanPurpose Purpose { get; set; }

        public virtual DateTime CreationDate
        {
            get;
            set;
        }

        public virtual string Language { get; set; }

        public virtual string Url
        {
            get;
            set;
        }

        public virtual TrainingType TrainingType
        {
            get;
            set;
        }

        public virtual DateTime? PublishDate
        {
            get; set;
        }

        public virtual ICollection<TrainingPlanDay> Days { get; set; }

        #endregion
    }
}