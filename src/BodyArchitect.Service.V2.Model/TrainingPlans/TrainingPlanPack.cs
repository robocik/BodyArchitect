using System;
using System.Collections.Generic;
using System.Linq;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    [Serializable]
    public class TrainingPlanPack
    {
        public TrainingPlanPack()
        {
            Exercises = new List<ExerciseDTO>();
        }

        public List<ExerciseDTO> Exercises { get; private set; }
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
                    if (entry.Exercise!=null && exercises.ContainsKey(entry.Exercise.GlobalId) && pack.Exercises.Where(x => x.GlobalId == entry.Exercise.GlobalId).Count() == 0) //!pack.Exercises.Contains(entry.Exercise.GlobalId))
                    {
                        pack.Exercises.Add(exercises[entry.Exercise.GlobalId]);
                    }
                }
            }
            return pack;
        }
    }
}
