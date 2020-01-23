using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    public enum CustomerVirtualCriteria
    {
        All,
        VirtualOnly,
        StandardOnly
    }
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class CustomerSearchCriteria
    {
        [DataMember]
        public Gender? Gender { get; set; }

        [DataMember]
        public CustomerVirtualCriteria CustomerVirtualCriteria { get; set; }
    }
}
