using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public enum CriteriaOperator
    {
        And,
        Or
    }

    public enum SearchSortOrder
    {
        Newest,
        HighestRating,
        Name
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetSupplementsCycleDefinitionsParam
    {
        public GetSupplementsCycleDefinitionsParam()
        {
            Supplements = new List<Guid>();
            SearchGroups=new List<WorkoutPlanSearchCriteriaGroup>();
            Languages = new List<string>();
            Purposes = new List<WorkoutPlanPurpose>();
            Difficults=new List<TrainingPlanDifficult>();
        }

        [DataMember]
        [NotNullValidator]
        public List<Guid> Supplements { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<WorkoutPlanPurpose> Purposes { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<TrainingPlanDifficult> Difficults { get; private set; }

        [DataMember]
        public CriteriaOperator SupplementsListOperator { get; set; }

        [DataMember]
        public SearchSortOrder SortOrder { get; set; }

        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public CanBeIllegalCriteria LegalCriteria { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<string> Languages { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<WorkoutPlanSearchCriteriaGroup> SearchGroups { get; private set; }

        [DataMember]
        public Guid? PlanId { get; set; }
    }
}
