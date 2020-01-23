using System;
using System.Collections.Generic;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Service.Model.TrainingPlans
{
    [Serializable]
    public class TrainingPlanPack
    {
        public TrainingPlanPack()
        {
            Exercises = new List<Guid>();
        }

        public List<Guid> Exercises { get; private set; }
        public TrainingPlan TrainingPlan { get; set; }

        public string Version
        {
            get { return "1.1"; }
        }

        public static TrainingPlanPack Create(TrainingPlan plan,IDictionary<Guid,ExerciseDTO> exercises)
        {
            if (plan == null)
            {
                throw new ArgumentNullException("plan");
            }
            TrainingPlanPack pack = new TrainingPlanPack();
            pack.TrainingPlan = plan;
            foreach (var day in pack.TrainingPlan.Days)
            {
                foreach (var entry in day.Entries)
                {
                    if (exercises.ContainsKey(entry.ExerciseId) && !pack.Exercises.Contains(entry.ExerciseId))
                    {
                        pack.Exercises.Add(entry.ExerciseId);
                    }
                }
            }
            return pack;
        }
    }
}
