using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model.WP7
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class TrialStatusInfo
    {
        [DataMember]
        public DateTime? TrialStarted { get; set; }

    }
}
