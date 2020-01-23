using System;
using System.Collections.Generic;


namespace BodyArchitect.Model.Old
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
        Published,
        PendingPublish
    }

    public interface IGlobalObject
    {
        Guid GlobalId
        {
            get;
            set;
        }
    }

    public interface IRatingable : IGlobalObject
    {
        float Rating { get; set; }
    }

    [Serializable]
    public class TrainingPlan : IRatingable
    {
        #region Persistent properties

        public virtual PublishStatus Status { get; set; }

        public virtual int DaysCount { get; set; }


        public virtual float Rating
        {
            get;
            set;
        }

        public virtual object Tag { get; set; }

        //[Property(NotNull = true, Unique = true)]
        //[ValidateIsUnique]
        public virtual Guid GlobalId
        {
            get; set;
        }

        //[Property(NotNull = true,Length=Constants.NameColumnLength)]
        //[ValidateNonEmpty(ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "TrainingPlan_Name_Empty")]
        //[ValidateLength(1, Constants.NameColumnLength, ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "TrainingPlan_Name_Length")]
        public virtual string Name
        {
            get;
            set;
        }

        public virtual int Version { get; set; }

        //[Property(NotNull = true,Length=Constants.NameColumnLength)]
        //[ValidateNonEmpty(ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "TrainingPlan_Author_Empty")]
        //[ValidateLength(1, Constants.NameColumnLength, ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "TrainingPlan_Author_Length")]
        public virtual string Author { get; set; }

        public virtual Profile Profile { get; set; }
        //[Property(NotNull = true)]
        public virtual TrainingPlanDifficult Difficult { get; set; }

        public virtual WorkoutPlanPurpose Purpose { get; set; }

        //[Property(NotNull = true)]
        public virtual DateTime CreationDate
        {
            get;
            set;
        }

        //[Property(SqlType = "ntext", ColumnType = "StringClob")]
        public virtual string PlanContent
        {
            get; set;
        }

        public virtual string Language { get; set; }

        //[Property(NotNull = true)]
        public virtual TrainingType TrainingType
        {
            get;
            set;
        }

        public virtual DateTime? PublishDate
        {
            get; set;
        }

        #endregion

        #region Methods

        //public TrainingPlan GetTrainingPlan()
        //{
        //    XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
        //    TrainingPlan newPlan =formatter.FromXml(PlanContent);

        //    return newPlan;
        //}

        //public void SetTrainingPlan(TrainingPlan plan)
        //{
        //    Name = plan.Name;
        //    Author = plan.Author;
        //    CreationDate = plan.CreationDate;
        //    TrainingType = plan.TrainingType;
        //    Version = plan.Version.ToString();
        //    Difficult = plan.Difficult;
        //    GlobalId = plan.GlobalId;
        //    Signed = plan.Signed;
        //    XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
        //    PlanContent = formatter.ToXml(plan).InnerXml;
        //}
        #endregion

        //#region Static methods

        //public  static IList<TrainingPlan> GetAll()
        //{
        //    return FindAll(typeof(TrainingPlan)).Cast<TrainingPlan>().ToList();
        //}
        //#endregion
    }
}