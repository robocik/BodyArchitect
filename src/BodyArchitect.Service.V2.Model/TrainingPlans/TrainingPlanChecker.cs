using System;
using System.Linq;


namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    public class TrainingPlanChecker
    {
        public void Process(TrainingPlan plan)
        {
            foreach (TrainingPlanDay day in plan.Days)
            {
                for (int i = day.Entries.Count - 1; i >= 0; i--)
                {
                    if(day.Entries[i].Exercise==null)
                    {
                        day.RemoveEntry(day.Entries[i]);
                    }
                }

                //get all entries with GroupName (superset)
                var entries=plan.Days.SelectMany(x=>x.Entries).Where(x=>!string.IsNullOrEmpty(x.GroupName)).ToList();

                foreach (var trainingPlanEntry in entries)
                {
                    if (plan.Days.SelectMany(x => x.Entries).Count(x => x.GroupName == trainingPlanEntry.GroupName) < 2)
                    {
                        trainingPlanEntry.GroupName = null;
                    }
                }
            }

        }
    }
}
