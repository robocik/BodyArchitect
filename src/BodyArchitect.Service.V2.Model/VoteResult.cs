﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class VoteResult
    {
        [DataMember]
        public float Rating { get; set; }
    }
}
