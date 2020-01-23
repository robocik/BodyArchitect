using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    public enum MessageOperationType
    {
        Delete
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MessageOperationParam
    {
        [DataMemberAttribute]
        public MessageOperationType Operation { get; set; }

        [DataMember]
        public int MessageId { get; set; }
    }
}
