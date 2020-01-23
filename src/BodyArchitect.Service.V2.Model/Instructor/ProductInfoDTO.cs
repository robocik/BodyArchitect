using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    public class ProductInfoDTO
    {
        [DataMember]
        public PaymentBaseDTO Payment { get; set; }

        [DataMember]
        public ProductDTO Product { get; set; }
    }
}
