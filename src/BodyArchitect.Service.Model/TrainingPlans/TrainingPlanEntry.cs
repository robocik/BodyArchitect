using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model.TrainingPlans
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [HasSelfValidation]
    public class TrainingPlanEntry : TrainingPlanBase, IRepositionableChild,ICommentable
    {
        public TrainingPlanEntry()
        {
            Sets = new List<TrainingPlanSerie>();
        }

        private Guid exerciseId;

        [DataMember]
        [ObjectCollectionValidator(typeof(TrainingPlanSerie))]
        public List<TrainingPlanSerie> Sets { get; private set; }

        [DataMember]
        public int RestSeconds { get; set; }

        [DataMember]
        [NotNullValidator]
        public TrainingPlanDay Day { get; internal set; }

        [DataMember]
        public Guid ExerciseId
        {
            get { return exerciseId; }
            set { exerciseId = value; }
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
            if (ExerciseId==Guid.Empty)
            {
                results.AddResult(new ValidationResult(ValidationStrings.TrainingPlanEntry_ExerciseId_NotSet, this, "ExerciseId", null, null));
            }
        }
    }
}
