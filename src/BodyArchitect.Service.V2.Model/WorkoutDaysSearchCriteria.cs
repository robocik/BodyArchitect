using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class WorkoutDaysSearchCriteria:IUserParameter
    {
        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }
    }
}
