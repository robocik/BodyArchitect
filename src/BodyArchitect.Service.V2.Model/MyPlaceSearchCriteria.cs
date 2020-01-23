using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum MyPlaceSortOrder
    {
        Name,
        CreationDate
    }
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MyPlaceSearchCriteria
    {
        public MyPlaceSortOrder SortOrder { get; set; }

        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public Guid? ProfileId { get; set; }
    }
}
