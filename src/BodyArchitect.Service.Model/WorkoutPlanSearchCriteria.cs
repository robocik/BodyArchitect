using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Service.Model.TrainingPlans;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum WorkoutPlanSearchCriteriaGroup
    {
        Mine,
        Favorites,
        Other
    }

    public enum WorkoutPlanSearchOrder
    {
        Newest,
        HighestRating
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class WorkoutPlanSearchCriteria
    {
        public WorkoutPlanSearchCriteria()
        {
            Languages = new List<string>();
            Difficults=new List<TrainingPlanDifficult>();
            Purposes = new List<WorkoutPlanPurpose>();
            WorkoutPlanType=new List<TrainingType>();
            SearchGroups = new List<WorkoutPlanSearchCriteriaGroup>();
            Days=new List<int>();
        }

        [DataMember]
        [NotNullValidator]
        public List<int> Days { get; set; }
        
        [DataMember]
        [NotNullValidator]
        public List<WorkoutPlanPurpose> Purposes { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<string> Languages { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<TrainingPlanDifficult> Difficults { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<TrainingType> WorkoutPlanType { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<WorkoutPlanSearchCriteriaGroup> SearchGroups { get; private set; }

        [DataMember]
        public WorkoutPlanSearchOrder SortOrder { get; set; }

        [DataMember]
        [ValidatorComposition(CompositionType.Or)]
        [StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "UserDTO_UserName_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        [NotNullValidator(Negated = true)]
        public string UserName
        {
            get; set;
        }

        public static WorkoutPlanSearchCriteria CreateFindAllCriteria()
        {
            WorkoutPlanSearchCriteria criteria=new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.AddRange((IEnumerable<WorkoutPlanSearchCriteriaGroup>)Enum.GetValues(typeof(WorkoutPlanSearchCriteriaGroup)));
            return criteria;
        }
    }
}
