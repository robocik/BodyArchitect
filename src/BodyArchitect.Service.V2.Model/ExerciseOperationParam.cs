using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ExerciseOperationParam
    {
        [DataMember]
        public Guid ExerciseId { get; set; }

        [DataMember]
        public FavoriteOperation Operation { get; set; }
    }
}
