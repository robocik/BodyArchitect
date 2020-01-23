using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class CustomerSettingsDTO:BAGlobalObject
    {
        [DataMember]
        public bool AutomaticUpdateMeasurements { get; set; }
    }
}
