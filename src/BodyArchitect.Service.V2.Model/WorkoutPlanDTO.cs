using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;


namespace BodyArchitect.Service.V2.Model
{
    public enum WorkoutPlanPurpose
    {
        NotSet,
        Strength,
        Mass,
        FatLost,
        Definition,
        Other
    }

    public enum PublishStatus
    {
        Private,
        Published
    }

    public interface IRatingable
    {
        Guid GlobalId{get;}
        float Rating { get; set; }
        float? UserRating { get; set; }
        string UserShortComment { get; set; }
    }
   
    public interface IBelongToUser
    {
        UserDTO Profile { get; set; }
    }

    //[Serializable]
    //[DataContract(Namespace = Const.ServiceNamespace)]
    //[DebuggerDisplay("Name = {Name}")]
    //public class WorkoutPlanDTO : BAGlobalObject, IRatingable, IBelongToUser, IExtensibleDataObject
    //{
    //    #region Persistent properties

    //    [DataMember]
    //    public PublishStatus Status { get; set; }

    //    /// <summary>
    //    /// UTC TIME
    //    /// </summary>
    //    [DataMember]
    //    public DateTime? PublishDate { get; set; }

    //    [DataMember]
    //    [LanguageValidator(MessageTemplateResourceName = "TrainingPlan_Language", MessageTemplateResourceType = typeof(ValidationStrings))]
    //    public string Language { get; set; }

    //    [DataMember]
    //    [RangeValidator(1, RangeBoundaryType.Inclusive, 7, RangeBoundaryType.Inclusive)]
    //    public int DaysCount
    //    {
    //        get;
    //        set;
    //    }

    //    public bool IsContentLoaded
    //    {
    //        get { return Content != null; }
    //    }

    //    [DataMember]
    //    public float Rating
    //    {
    //        get;
    //        set;
    //    }

    //    [DataMember]
    //    public TrainingPlan Content
    //    {
    //        get; set; }

    //    [DataMember]
    //    public string UserShortComment { get; set; }

    //    [DataMember]
    //    public float? UserRating
    //    {
    //        get;
    //        set;
    //    }

    //    [DataMember]
    //    [NotNullValidator]
    //    public UserDTO Profile { get; set; }

    //    [DataMember]
    //    [DoNotChecksum]
    //    [SerializerId]
    //    public int Version
    //    {
    //        get;
    //        set;
    //    }

    //    [DataMember]
    //    [NotNullValidator(MessageTemplateResourceName = "TrainingPlan_Name_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
    //    [StringLengthValidator(1, Constants.NameColumnLength, MessageTemplateResourceName = "TrainingPlan_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
    //    public string Name
    //    {
    //        get;
    //        set;
    //    }

    //    [DataMember]
    //    [ValidatorComposition(CompositionType.Or)]
    //    [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "TrainingPlan_Author_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
    //    [NotNullValidator(Negated = true)]
    //    public string Author { get; set; }

    //    [DataMember]
    //    public WorkoutPlanPurpose Purpose { get; set; }

    //    [DataMember]
    //    public TrainingPlanDifficult Difficult { get; set; }

    //    /// <summary>
    //    /// UTC TIME
    //    /// </summary>
    //    [DataMember]
    //    public DateTime CreationDate
    //    {
    //        get;
    //        set;
    //    }

    //    [DataMember]
    //    public TrainingType TrainingType
    //    {
    //        get;
    //        set;
    //    }
    //    #endregion

    //    #region Methods

    //    public void SetTrainingPlan(TrainingPlan plan)
    //    {
    //        Name = plan.Name;
    //        Author = plan.Author;
    //        CreationDate = plan.CreationDate;
    //        TrainingType = plan.TrainingType;
    //        Difficult = plan.Difficult;
    //        GlobalId = plan.GlobalId;
    //        DaysCount = plan.Days.Count;
    //        Language = plan.Language;
    //        Purpose = plan.Purpose;
    //        Content = plan;
    //    }
    //    #endregion
    //}
}