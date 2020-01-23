using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum VoteObject
    {
        Exercise,
        WorkoutPlan,
        Supplement,
        SupplementCycleDefinition
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class VoteParams
    {
        [DataMember]
        public VoteObject ObjectType { get; set; }

        [DataMember]
        public Guid GlobalId { get; set; }

        [DataMember]
        public float? UserRating { get; set; }

        [DataMember]
        public string UserShortComment { get; set; }
    }
}
