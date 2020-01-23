using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class SaveChampionshipResult
    {
        [DataMember]
        public ChampionshipDTO Championship { get; set; }

        [DataMember]
        public IList<SerieDTO> NewRecords { get; set; }
    }
}
