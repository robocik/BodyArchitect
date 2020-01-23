using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Reports
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ExerciseRecordsReportResultItem
    {
        [DataMember]
        public DateTime TrainingDate { get; set; }

        [DataMember]
        public Guid SerieId { get; set; }

        [DataMember]
        public decimal MaxWeight { get; set; }

        [DataMember]
        public int Repetitions { get; set; }

        [DataMember]
        public UserDTO User { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }

        [DataMember]
        public ExerciseLightDTO Exercise { get; set; }
    }
}
