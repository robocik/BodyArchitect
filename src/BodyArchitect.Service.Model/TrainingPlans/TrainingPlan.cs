using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Service.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model.TrainingPlans
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    [HasSelfValidation]
    public class TrainingPlan : TrainingPlanBase, ICommentable, IRepositionableParent
    {
        private TrainingType trainingType;
        
        public TrainingPlan()
        {
            Days=new List<TrainingPlanDay>();
            CreationDate = DateTime.UtcNow;
        }

        [DataMember]
        [LanguageValidator(MessageTemplateResourceName = "TrainingPlan_Language", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Language { get; set; }

        [DataMember]
        [ObjectCollectionValidator(typeof(TrainingPlanDay))]
        public List<TrainingPlanDay> Days { get; private set; }


        [DataMember]
        public WorkoutPlanPurpose Purpose { get; set; }

        public object Tag { get; set; }

        [DataMember]
        public Guid? BasedOnId { get; set; }

        //[ValidateNonEmpty(ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "TrainingPlan_Name_Empty")]
        //[ValidateLength(1, Constants.NameColumnLength, ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "TrainingPlan_Name_Length")]
        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "TrainingPlan_Name_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(1, Constants.NameColumnLength, MessageTemplateResourceName = "TrainingPlan_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Name { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "TrainingPlan_Author_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Author { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.UrlLength, MessageTemplateResourceName = "TrainingPlan_Url_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string Url { get; set; }

        [DataMember]
        public DateTime CreationDate { get; internal set; }

        [DataMember]
        public TrainingPlanDifficult Difficult { get; set; }

        [DataMember]
        public string Comment
        {
            get;
            set;
        }

        [DataMember]
        public int RestSeconds { get; set; }

        [DataMember]
        public TrainingType TrainingType
        {
            get { return trainingType; }
            set { trainingType = value; }
        }

        public TrainingPlanDay GetDay(Guid id)
        {
            return (from i in Days where i.GlobalId == id select i).SingleOrDefault();
        }

        public void AddDay(TrainingPlanDay entry)
        {
            entry.TrainingPlan = this;
            Days.Add(entry);

        }

        public void RemoveDay(TrainingPlanDay entry)
        {
            Days.Remove(entry);
            entry.TrainingPlan = null;
        }

        public int GetMaximumSeriesCount()
        {
            int max = 0;
            foreach (var entry in Days)
            {
                foreach (var trainingPlanEntry in entry.Entries)
                {
                    if (max < trainingPlanEntry.Sets.Count)
                    {
                        max = trainingPlanEntry.Sets.Count;
                    }
                }
            }
            return max;
        }

        public virtual void RepositionEntry(int index1, int index2)
        {
            var item = Days[index1];
            Days.Remove(item);
            Days.Insert(index2, item);
        }

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if(Days.Count==0)
            {
                results.AddResult(new ValidationResult(ValidationStrings.TrainingPlan_Days_Empty, this, "Days", null, null));
            }
            var result = from day in Days group day by day.Name into g select g;
            var dict = result.Select(t => t.ToList().Count > 1).ToList().IndexOf(true);
            if (dict > -1)
            {
                results.AddResult(new ValidationResult(ValidationStrings.TrainingPlan_DaysName_Unique,this,"Days",null,null));
            }
        }

    }

    

    

    
}
