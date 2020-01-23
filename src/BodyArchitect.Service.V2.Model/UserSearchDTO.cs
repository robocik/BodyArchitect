using System;
using System.Runtime.Serialization;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class UserSearchDTO : UserDTO
    {
        [DataMember]
        public ProfileStatisticsDTO Statistics { get; set; }

        [DataMember]
        public bool IsOnline { get; set; }
    }
}
