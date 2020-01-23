using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum SupplementsCycleDefinitionOperation
    {
        Delete,
        AddToFavorites,
        RemoveFromFavorites,
        Publish
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class SupplementsCycleDefinitionOperationParam
    {
        [DataMember]
        public SupplementsCycleDefinitionOperation Operation { get; set; }

        [DataMember]
        public Guid SupplementsCycleDefinitionId { get; set; }
    }
}
