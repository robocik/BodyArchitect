using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetChampionshipsCriteria
    {
        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public Guid? ChampionshipId { get; set; }
    }
}
