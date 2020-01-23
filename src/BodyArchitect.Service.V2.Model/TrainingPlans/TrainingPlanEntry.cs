using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [HasSelfValidation]
    public class TrainingPlanEntry : BAGlobalObject, IRepositionableChild, ICommentable
    {
        public TrainingPlanEntry()
        {
            Sets = new List<TrainingPlanSerie>();
        }

        private ExerciseLightDTO exercise;

        [DataMember]
        [ObjectCollectionValidator(typeof(TrainingPlanSerie))]
        public List<TrainingPlanSerie> Sets { get; set; }

        [DataMember]
        public ExerciseDoneWay DoneWay
        {
            get;
            set;
        }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public int? RestSeconds { get; set; }

        [DataMember]
        [NotNullValidator]
        public TrainingPlanDay Day { get; internal set; }

        [DataMember]
        [SkipCloneable]
        public ExerciseLightDTO Exercise
        {
            get { return exercise; }
            set { exercise = value; }
        }

        public int Position { get { return Day.Entries.IndexOf(this); }}

        IRepositionableParent IRepositionableChild.RepositionableParent
        {
            get { return Day; }
        }

        [DataMember]
        public string Comment
        {
            get; set;
        }

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if (Exercise==null)
            {
                results.AddResult(new ValidationResult(ValidationStrings.TrainingPlanEntry_ExerciseId_NotSet, this, "Exercise", null, null));
            }
        }
    }
}
