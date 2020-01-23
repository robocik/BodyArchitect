using System;


namespace BodyArchitect.Service.Model.TrainingPlans
{
    public class TrainingPlanChecker
    {
        public void Process(TrainingPlan plan)
        {
            foreach (TrainingPlanDay day in plan.Days)
            {
                for (int i = day.Entries.Count - 1; i >= 0; i--)
                {
                    if(day.Entries[i].ExerciseId==Guid.Empty)
                    {
                        day.RemoveEntry(day.Entries[i]);
                    }
                }

                for (int i = day.SuperSets.Count - 1; i >= 0; i--)
                {
                    if(day.SuperSets[i].SuperSets.Count<2)
                    {
                        day.SuperSets.RemoveAt(i);
                    }
                }
            }

        }
    }
}
