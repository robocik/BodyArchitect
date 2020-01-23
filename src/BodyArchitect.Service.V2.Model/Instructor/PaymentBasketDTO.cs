using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    public class PaymentBasketDTO:BAGlobalObject
    {
        public PaymentBasketDTO()
        {
            Payments = new List<PaymentDTO>();
            DateTime = DateTime.UtcNow;
        }


        [DataMember]
        public Guid CustomerId { get; set; }
        
        [DataMember]
        public decimal TotalPrice { get; set; }

        [DataMember]
        public Guid ProfileId { get; set; }

        [DataMember]
        [ObjectCollectionValidator(typeof(PaymentDTO))]
        public IList<PaymentDTO> Payments { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }
    }
}
