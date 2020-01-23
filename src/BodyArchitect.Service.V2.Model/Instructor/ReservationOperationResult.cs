using System;
using System.Runtime.Serialization;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ReservationOperationResult
    {
        [DataMember]
        public ScheduleEntryReservationDTO Reservation { get; set; }

        [DataMember]
        public ScheduleEntryBaseDTO ScheduleEntry { get; set; }

        [DataMember]
        public PaymentDTO Payment { get; set; }
    }
}