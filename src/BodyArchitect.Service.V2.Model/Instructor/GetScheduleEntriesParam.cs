using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetScheduleEntriesParam
    {
        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public DateTime? EndTime { get; set; }

        [DataMember]
        public Guid? ActivityId { get; set; }

        [DataMember]
        public Guid? EntryId { get; set; }


    }
}
