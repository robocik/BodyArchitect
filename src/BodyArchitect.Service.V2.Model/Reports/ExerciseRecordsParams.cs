using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Reports
{
    public enum RecordMode
    {
        AllUsers,
        Friends,
        Customer
    }
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ExerciseRecordsParams
    {
        [DataMember]
        public Guid ExerciseId { get; set; }

        [DataMember]
        public RecordMode Mode { get; set; }
    }
}
