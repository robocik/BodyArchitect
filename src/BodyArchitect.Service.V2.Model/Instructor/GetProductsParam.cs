using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    public enum ProductsSortOrder
    {
        ByName,
        ByPaid,
        ByDate
    }

    public enum PaymentCriteria
    {
        Any,
        WithPayment,
        WithoutPayment,
    }

    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetProductsParam
    {
        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public DateTime? EndTime { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }

        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public ProductsSortOrder SortOrder { get; set; }

        [DataMember]
        public PaymentCriteria PaymentCriteria { get; set; }
    }
}
