using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Reports
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class WeightReperitionReportResultItem
    {
        [DataMember]
        public decimal Repetitions { get; set; }

        [DataMember]
        public decimal Weight { get; set; }

        [DataMember]
        public Guid StrengthTrainingItemId { get; set; }

        [DataMember]
        public ExerciseLightDTO Exercise { get; set; }
    }
}
