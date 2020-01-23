using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Reports
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MeasurementsTimeReportResultItem
    {
        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public Guid SizeEntryId { get; set; }

        [DataMember]
        public WymiaryDTO Wymiary { get; set; }
    }
}
