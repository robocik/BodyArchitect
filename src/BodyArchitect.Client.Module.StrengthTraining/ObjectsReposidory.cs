using System;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining
{
    public class WorkoutPlansReposidory : ObjectsCacheBase<TrainingPlan>
    {
        private static WorkoutPlansReposidory instance;
        public static WorkoutPlansReposidory Instance
        {
            get
            {
                if(instance==null)
                {
                    instance=new WorkoutPlansReposidory();
                }
                return instance;
            } 
        }

        protected override PagedResult<TrainingPlan> GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
            WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
            criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
            return ServiceManager.GetWorkoutPlans(criteria, pageInfo);
        }
    }

    public class ExercisesReposidory:ObjectsCacheBase<ExerciseDTO>
    {
        private static ExercisesReposidory instance;
        public static ExercisesReposidory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ExercisesReposidory();
                }
                return instance;
            }
        }

        public ExerciseDTO BenchPress
        {
            get { return GetItem(new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4")); }
        }

        public ExerciseDTO Deadlift
        {
            get { return GetItem(new Guid("505988e1-5663-41f1-aa1a-9b92ea584263")); }
        }

        public ExerciseDTO Sqad
        {
            get { return GetItem(new Guid("3e06a130-b811-4e45-9285-f087403615bf")); }
        }

        public override ExerciseDTO GetItem(Guid id)
        {
            var item= base.GetItem(id);
            if(item==null)
            {
                return ExerciseDTO.Removed;
            }
            return item;
        }

        protected override PagedResult<ExerciseDTO>  GetItemsMethod(PartialRetrievingInfo pageInfo)
        {
 	        ExerciseSearchCriteria search = ExerciseSearchCriteria.CreatePersonalCriteria();
            return ServiceManager.GetExercises(search, pageInfo);
        }
    }
}
