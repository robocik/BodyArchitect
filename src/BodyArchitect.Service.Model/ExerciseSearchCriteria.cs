using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum ExerciseSearchCriteriaGroup
    {
        Mine,
        //Favorites,
        //Other,
        PendingPublish,
        Global
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ExerciseSearchCriteria
    {
        public ExerciseSearchCriteria()
        {
            SearchGroups = new List<ExerciseSearchCriteriaGroup>();
            ExerciseTypes=new List<ExerciseType>();
        }
        [DataMember]
        public int? UserId { get; set; }

        [DataMember]
        public WorkoutPlanSearchOrder SortOrder { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<ExerciseSearchCriteriaGroup> SearchGroups { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<ExerciseType> ExerciseTypes { get; private set; }

        public static ExerciseSearchCriteria CreateAllCriteria()
        {
            ExerciseSearchCriteria criteria = new ExerciseSearchCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Global);
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.PendingPublish);
            return criteria;
        }

        public static ExerciseSearchCriteria CreatePersonalCriteria()
        {
            ExerciseSearchCriteria criteria = new ExerciseSearchCriteria();
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Global);
            return criteria;
        }
    }
}
