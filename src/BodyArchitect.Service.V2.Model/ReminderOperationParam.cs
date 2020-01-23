using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum ReminderOperationType
    {
        Delete,
        CloseAfterShow
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ReminderOperationParam
    {
        [DataMember]
        public ReminderOperationType Operation { get; set; }

        [DataMember]
        public Guid ReminderItemId { get; set; }
    }
}
