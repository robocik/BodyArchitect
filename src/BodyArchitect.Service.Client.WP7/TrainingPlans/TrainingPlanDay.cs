using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{

    public class TrainingPlanDay : TrainingPlanBase
    {
        public List<TrainingPlanEntry> Entries { get; set; }

        public List<SuperSet> SuperSets { get; set; }

        public int Position
        { 
            get
            {
                if (plan != null)
                {
                    return plan.Days.IndexOf(this);
                }
                return -1;
            }
            set { }
        }

        private TrainingPlan plan;

        public TrainingPlanDay()
        {
            Entries = new List<TrainingPlanEntry>();
            SuperSets = new List<SuperSet>();
        }

        public string Name
        {
            get; set; 
        }

        public SuperSet GetSuperSet(TrainingPlanEntry planEntry)
        {
            var res = from set in SuperSets where set.SuperSets.Contains(planEntry) select set;
            return res.SingleOrDefault();
        }

        public TrainingPlanEntry GetEntry(Guid globalId)
        {
            foreach (var trainingPlanEntry in Entries)
            {
                if (trainingPlanEntry.GlobalId == globalId)
                {
                    return trainingPlanEntry;
                }
            }
            return null;
        }

        public TrainingPlan TrainingPlan
        {
            get { return plan; }
            set { plan = value; }
        }

        public void AddEntry(TrainingPlanEntry entry)
        {
            Entries.Add(entry);
            entry.Day = this;
        }

        public void RemoveEntry(TrainingPlanEntry entry)
        {
            Entries.Remove(entry);
            entry.Day = null;
        }

        public void RepositionEntry(int index1, int index2)
        {
            var item = Entries[index1];
            Entries.Remove(item);
            Entries.Insert(index2, item);
        }


    }
}
