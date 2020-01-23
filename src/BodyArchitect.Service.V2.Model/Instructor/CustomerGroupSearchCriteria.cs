using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Instructor
{
    public enum CustomerGroupMembersCriteria
    {
        All,
        WithMembersOnly,
        WithoutMembersOnly
    }
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class CustomerGroupSearchCriteria
    {
        [DataMember]
        public CustomerGroupMembersCriteria MembersCriteria { get; set; }
    }
}
