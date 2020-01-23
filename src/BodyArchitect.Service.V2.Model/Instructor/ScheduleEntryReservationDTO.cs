using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    public class ScheduleEntryReservationDTO : ProductDTO
    {

        [DataMember]
        public DateTime? EnterDateTime { get; set; }

        [DataMember]
        public DateTime? LeaveDateTime { get; set; }

        [DataMember]
        public Guid ScheduleEntryId { get; set; }

        public bool IsPresent
        {
            get { return EnterDateTime.HasValue; }
        }
    }
}
