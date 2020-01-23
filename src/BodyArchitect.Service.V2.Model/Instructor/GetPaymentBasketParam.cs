using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespaceRoot)]
    public class GetPaymentBasketParam
    {
        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public DateTime? EndTime { get; set; }

        [DataMember]
        public Guid? CustomerId
        {
            get;
            set;
        }

    }
}
