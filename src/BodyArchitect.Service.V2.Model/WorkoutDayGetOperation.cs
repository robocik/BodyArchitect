using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class WorkoutDayGetOperation
    {
        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }

        [DataMember]
        public GetOperation Operation { get; set; }

        [DataMember]
        public DateTime? WorkoutDateTime { get; set; }
    }
}
