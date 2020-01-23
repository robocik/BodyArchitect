using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public enum WorkoutPlanSearchCriteriaGroup
    {
        Mine,
        Favorites,
        Other
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
        public Guid? PlanId { get; set; }

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
        public bool SortAscending { get; set; }

        [DataMember]
        public SearchSortOrder SortOrder { get; set; }

        [DataMember]
        public Guid? UserId { get; set; }

    }
}
