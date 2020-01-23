using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetFeaturedDataParam
    {
        [DataMember]
        public int? LatestTrainingPlansCount { get; set; }

        [DataMember]
        public int? LatestSupplementsDefinitionsCount { get; set; }

        [DataMember]
        public int? RandomWorkoutPlansCount { get; set; }

        [DataMember]
        public int? RandomSupplementsDefinitionsCount { get; set; }

        [DataMember]
        public int? LatestBlogsCount { get; set; }

        [DataMember]
        public int? LatestStrengthTrainingsCount { get; set; }

        [DataMember]
        public bool SkipRecords { get; set; }
    }
}
