using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class RetrievingInfo
    {
        public RetrievingInfo()
        {
            Images = true;
            LongTexts = true;
        }

        [DataMember]
        public bool Images { get; set; }

        [DataMember]
        public bool LongTexts { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class PartialRetrievingInfo : RetrievingInfo
    {
        public const int DefaultPageSize = 100;
        public const int AllElementsPageSize = 0;
        public const int LastPageIndex = -1;

        public PartialRetrievingInfo()
        {
            PageSize = DefaultPageSize;
        }

        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public int PageSize { get; set; }
    }
}
