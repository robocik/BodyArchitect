using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum ProfileOperation
    {
        Delete,
        SetStatus,
        AccountType
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ProfileOperationParam
    {
        [DataMember]
        public ProfileOperation Operation { get; set; }

        [DataMember]
        public Guid ProfileId { get; set; }

        [DataMember]
        public ProfileStatusDTO Status { get; set; }

        [DataMember]
        public AccountType? AccountType { get; set; }
    }
}
