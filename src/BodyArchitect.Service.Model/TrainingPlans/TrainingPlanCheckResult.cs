using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.Model.TrainingPlans
{
    public class TrainingPlanCheckResult
    {
        List<TrainingPlanCheckItem> result = new List<TrainingPlanCheckItem>();

        public void AddResult(TrainingPlanBase planItem, string resourceKey, TrainingPlanCheckItemStatus status)
        {
            var item = new TrainingPlanCheckItem(planItem, resourceKey, status);
            AddResult(item);
        }

        public void AddResult(TrainingPlanCheckItem item)
        {
            result.Add(item);
        }

        public ReadOnlyCollection<TrainingPlanCheckItem> Results
        {
            get { return result.AsReadOnly(); }
        }
    }

    public enum TrainingPlanCheckItemStatus
    {
        Information,
        Warning,
        Error
    }

    public class TrainingPlanCheckItem
    {
        private TrainingPlanBase trainingPlanBase;
        private string resourceKey;
        private TrainingPlanCheckItemStatus status;

        public TrainingPlanCheckItem(TrainingPlanBase trainingPlanBase, string resourceKey,TrainingPlanCheckItemStatus status)
        {
            this.trainingPlanBase = trainingPlanBase;
            this.resourceKey = resourceKey;
            this.Status = status;
        }

        public TrainingPlanBase TrainingPlanBase
        {
            get { return trainingPlanBase; }
            set { trainingPlanBase = value; }
        }

        public string ResourceKey
        {
            get { return resourceKey; }
            set { resourceKey = value; }
        }

        public TrainingPlanCheckItemStatus Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
