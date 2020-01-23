using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class FeaturedData
    {
        public FeaturedData()
        {
            LatestTrainingPlans = new List<TrainingPlan>();
            RandomTrainingPlans=new List<TrainingPlan>();
            RandomSupplementsDefinitions=new List<SupplementCycleDefinitionDTO>();
            SupplementsDefinitions=new List<SupplementCycleDefinitionDTO>();
            LatestBlogs = new List<FeaturedEntryObjectDTO>();
            LatestStrengthTrainings = new List<FeaturedEntryObjectDTO>();
            Records=new List<ExerciseRecordsReportResultItem>();

        }

        [DataMember]
        public IList<TrainingPlan> RandomTrainingPlans { get; set; }

        [DataMember]
        public IList<TrainingPlan> LatestTrainingPlans { get; set; }

        [DataMember]
        public IList<SupplementCycleDefinitionDTO> RandomSupplementsDefinitions { get; set; }

        [DataMember]
        public IList<SupplementCycleDefinitionDTO> SupplementsDefinitions { get; set; }

        [DataMember]
        public IList<FeaturedEntryObjectDTO> LatestBlogs { get; set; }

        [DataMember]
        public IList<FeaturedEntryObjectDTO> LatestStrengthTrainings { get; set; }

        [DataMember]
        public IList<ExerciseRecordsReportResultItem> Records { get; set; }
    }
}
