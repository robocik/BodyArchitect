using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    [KnownType(typeof(PaymentDTO))]
    public abstract class PaymentBaseDTO:BAGlobalObject
    {
        [DataMember]
        [ObjectValidator]
        public ProductDTO Product { get; set; }

        //UTC
        [DataMember]
        public DateTime DateTime { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class PaymentDTO : PaymentBaseDTO
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public Guid PaymentBasketId { get; set; }
    }
}
