using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;


namespace BodyArchitect.Service.Model.TrainingPlans
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace,IsReference = true)]
    [HasSelfValidation]
    public class TrainingPlanDay : TrainingPlanBase, IRepositionableChild,IRepositionableParent 
    {
        [DataMember]
        [ObjectCollectionValidator(typeof(TrainingPlanEntry))]
        public List<TrainingPlanEntry> Entries { get; private set; }

        [DataMember]
        [ObjectCollectionValidator(typeof(SuperSet))]
        public List<SuperSet> SuperSets { get; private set; }

        public int Position { get { return plan.Days.IndexOf(this); } }

        [DataMember]
        [NotNullValidator]
        private TrainingPlan plan;

        public TrainingPlanDay()
        {
            Entries = new List<TrainingPlanEntry>();
            SuperSets = new List<SuperSet>();
        }

        [DataMember]
        [NotNullValidator]
        [StringLengthValidator(1, Constants.NameColumnLength, MessageTemplateResourceName = "TrainingPlanDay_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
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

        [DataMember]
        [NotNullValidator]
        public TrainingPlan TrainingPlan
        {
            get { return plan; }
            internal set { plan = value; }
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

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if (Entries.Count == 0)
            {
                results.AddResult(new ValidationResult(ValidationStrings.TrainingPlan_Entries_Empty, this, "Entries", null, null));
            }
        }
        #region IRepositionableChild Members

        
        IRepositionableParent IRepositionableChild.RepositionableParent
        {
            get { return plan; }
        }

        #endregion

    }
}
