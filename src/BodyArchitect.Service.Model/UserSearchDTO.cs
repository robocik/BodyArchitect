using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class UserSearchDTO : UserDTO
    {
        [DataMember]
        public ProfileStatisticsDTO Statistics { get; set; }
    }
}
