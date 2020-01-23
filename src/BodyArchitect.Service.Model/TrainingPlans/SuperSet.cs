using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model.TrainingPlans
{
    [Serializable]
    [HasSelfValidation]
    [DataContract(Namespace = Const.ServiceNamespace)]
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

        [DataMember]
        public List<TrainingPlanEntry> SuperSets
        {
            get { return superSets; }
        }

        [DataMember]
        public Guid SuperSetId
        {
            get { return superSetId; }
            internal set { superSetId = value; }
        }

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if (SuperSets.Count < 2 || SuperSets[0]==SuperSets[1])
            {
                results.AddResult(new ValidationResult(ValidationStrings.SuperSet_Empty, this, "SuperSets", null, null));
            }
            
        }
    }
}
