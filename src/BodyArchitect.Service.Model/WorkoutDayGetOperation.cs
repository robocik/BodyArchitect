using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class WorkoutDayGetOperation
    {
        [DataMember]
        public int? UserId { get; set; }
        [DataMember]
        public GetOperation Operation { get; set; }
        [DataMember]
        public DateTime? WorkoutDateTime { get; set; }
    }
}
