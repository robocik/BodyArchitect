using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum MyPlaceOperationType
    {
        Delete,
        SetDefault
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MyPlaceOperationParam
    {
        [DataMember]
        public Guid MyPlaceId { get; set; }

        [DataMember]
        public MyPlaceOperationType Operation { get; set; }
    }
}
