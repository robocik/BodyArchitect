using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum SupplementProductCriteria
    {
        All,
        OnlyGeneral,
        OnlyProducts
    }

    public enum CanBeIllegalCriteria
    {
        All,
        OnlyLegal,
        OnlyIllegal
    }
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetSupplementsParam
    {
        [DataMember]
        public SearchSortOrder SortOrder { get; set; }

        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public SupplementProductCriteria ProductCriteria { get; set; }

        [DataMember]
        public CanBeIllegalCriteria LegalCriteria { get; set; }
    }
}
