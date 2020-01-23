using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Reports
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ReportMeasurementsTimeParams : IUserParameter
    {
        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public bool UseAllEntries { get; set; }

        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }
    }
}
