using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.Admin.Objects
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class DeleteOldProfilesParam
    {
        [DataMember]
        public bool OnlyShowUsers { get; set; }
    }
}
