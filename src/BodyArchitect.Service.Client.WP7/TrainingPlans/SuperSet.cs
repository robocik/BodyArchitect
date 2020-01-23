using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    public class SuperSet
    {
        private List<TrainingPlanEntry> superSets = new List<TrainingPlanEntry>();
        private Guid superSetId = Guid.NewGuid();

        public SuperSet()
        {

        }

        public SuperSet(params TrainingPlanEntry[] entries)
        {
            SuperSets.AddRange(entries);
        }

        public SuperSet(TrainingPlanEntry entry1, TrainingPlanEntry entry2)
        {
            SuperSets.Add(entry1);
            SuperSets.Add(entry2);
        }

        public List<TrainingPlanEntry> SuperSets
        {
            get { return superSets; }
            set { superSets = value; }
        }

        public Guid SuperSetId
        {
            get { return superSetId; }
            set { superSetId = value; }
        }

    }
}
