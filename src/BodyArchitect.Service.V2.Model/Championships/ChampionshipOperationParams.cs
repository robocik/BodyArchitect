﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum ChampionshipOperationType
    {
        Start    
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ChampionshipOperationParams
    {
        [DataMember]
        public Guid ChampionshipId { get; set; }

        [DataMember]
        public ChampionshipOperationType Operation { get; set; }
    }
}
