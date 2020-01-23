using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum MyTrainingOperationType
    {
        Start,
        Stop,
        Simulate
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MyTrainingOperationParam
    {
        [DataMember]
        public MyTrainingOperationType Operation { get; set; }

        [DataMember]
        public MyTrainingDTO MyTraining { get; set; }
    }


}
