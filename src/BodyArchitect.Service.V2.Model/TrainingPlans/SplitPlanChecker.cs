using System;
using System.Collections.Generic;
using System.Linq;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    public class SplitPlanChecker
    {
        private TrainingPlanPack pack;
        private IDictionary<Guid, ExerciseDTO> exercisesPack;

        public SplitPlanChecker(TrainingPlanPack pack)
        {
            this.pack = pack;
        }

        public TrainingPlanCheckResult Check(IDictionary<Guid, ExerciseDTO> exercisesPack)
        {
            if(pack.TrainingPlan==null)
            {
                throw new ArgumentNullException("TrainingPlan cannot be null");
            }
            if (exercisesPack==null)
            {
                throw new ArgumentNullException("exercisesPack");
            }
            this.exercisesPack = exercisesPack;
            TrainingPlanCheckResult result = new TrainingPlanCheckResult();

            check(pack.TrainingPlan, result);
            return result;
        }

        void check(TrainingPlan plan,TrainingPlanCheckResult result)
        {
            if(plan.RestSeconds==0)
            {
                result.AddResult(plan,"TrainingPlan_RestSecond_Empty",TrainingPlanCheckItemStatus.Information);
            }
            if(string.IsNullOrEmpty(plan.Name))
            {
                result.AddResult(plan, "TrainingPlan_Name_Empty", TrainingPlanCheckItemStatus.Error );
            }
            //not unique names
            checkUniquenessDayNames(plan, result);

            foreach (var day in plan.Days)
            {
                check(day, result);
            }
        }

        private void checkUniquenessDayNames(TrainingPlan plan, TrainingPlanCheckResult result)
        {
            var uniqueNamesResult = from day in plan.Days group day by day.Name into g select g;
            var moreResult=uniqueNamesResult.Where(t => t.ToList().Count > 1);
            foreach (var item in moreResult)
            {
                var test = item.ToList();
                for (int i = 1; i < test.Count; i++)
                {
                    result.AddResult(test[i], "TrainingPlan_DaysName_Unique", TrainingPlanCheckItemStatus.Error);
                }
                
            }
        }

        void check(TrainingPlanDay day, TrainingPlanCheckResult result)
        {
            if(string.IsNullOrEmpty(day.Name))
            {
                result.AddResult(day, "TrainingPlanDay_Name_Empty", TrainingPlanCheckItemStatus.Error);
            }
            //isolation before compoud should be check only in split plan (for now at least)
            if (day.TrainingPlan.TrainingType == TrainingType.Split)
            {
                isolationBeforeCompoudCheck(day, result);
            }
            if(day.Entries.Count==0)
            {
                result.AddResult(day,"TrainingPlan_EmptyDay",TrainingPlanCheckItemStatus.Information);
            }
            foreach (var planEntry in day.Entries)
            {
                check(planEntry,result);
            }
        }

        void isolationBeforeCompoudCheck(TrainingPlanDay day, TrainingPlanCheckResult result)
        {
            var musleSplit = from e1 in day.Entries
                             join exercise in this.exercisesPack.Values on e1.Exercise.GlobalId equals exercise.GlobalId
                             where pack.Exercises.Contains(e1.Exercise)
                             group e1 by exercise.ExerciseType into g
                             select g;

            foreach (var entriesForMuscle in musleSplit)
            {
                List<TrainingPlanEntry> isolatedExercises = new List<TrainingPlanEntry>();
                foreach (var entry in entriesForMuscle)
                {

                    ExerciseDTO exercise = exercisesPack[entry.Exercise.GlobalId];
                    if (exercise.MechanicsType == MechanicsType.Isolation)
                    {
                        isolatedExercises.Add(entry);
                    }
                    else if (exercise.MechanicsType == MechanicsType.Compound)
                    {
                        foreach (var isolatedExercise in isolatedExercises)
                        {
                            result.AddResult(isolatedExercise, "Split_IsolationExerciseBeforeCompoud", TrainingPlanCheckItemStatus.Information);
                        }
                        isolatedExercises.Clear();
                    }
                }

            }

           
        }

        void check(TrainingPlanEntry entry, TrainingPlanCheckResult result)
        {
            if(!pack.Exercises.Contains(entry.Exercise))
            {
                result.AddResult(entry, "TrainingPlan_ExerciseDoesntExist", TrainingPlanCheckItemStatus.Warning);
            }
            for (int i = 0; i < entry.Sets.Count; i++)
            {
                var set = entry.Sets[i];
                if (set.DropSet != DropSetType.None && entry.Sets.IndexOf(set)<entry.Sets.Count-1)
                {
                    result.AddResult(set, "TrainingPlan_DropSetInNotOnlyLastSet", TrainingPlanCheckItemStatus.Information);
                }
            }
        }
    }
}
