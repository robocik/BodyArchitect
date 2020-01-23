using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    public enum ReservationsOperationType
    {
        Make,
        MakeGroup,
        Undo,
        Presnet,
        Absent,
        StatusDone,
        StatusCancelled
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ReservationsOperationParam
    {
        [DataMember]
        public Guid? CustomerId { get; set; }

        [DataMember]
        public ReservationsOperationType OperationType { get; set; }

        [DataMember]
        public Guid? EntryId { get; set; }

        [DataMember]
        public Guid? ReservationId { get; set; }

        
    }
}
