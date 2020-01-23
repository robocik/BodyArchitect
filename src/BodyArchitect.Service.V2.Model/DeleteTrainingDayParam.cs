using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum DeleteTrainingDayMode
    {
        All,
        OnlyWithoutMyTraining
    }
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class DeleteTrainingDayParam
    {
        [DataMember]
        public Guid TrainingDayId { get; set; }

        [DataMember]
        public DeleteTrainingDayMode Mode { get; set; }
    }
}
