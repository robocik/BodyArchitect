using System;
using System.Runtime.Serialization;

namespace BodyArchitect.Service.V2.Model.Reports
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class WeightExerciseReportResultItem
    {
        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public ExerciseLightDTO Exercise { get; set; }

        [DataMember]
        public Guid StrengthTrainingItemId { get; set; }

        [DataMember]
        public decimal Weight { get; set; }
    }
}
