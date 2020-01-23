using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class FeaturedEntryObjectDTO:BAGlobalObject
    {
        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public UserDTO User { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }
    }
}
