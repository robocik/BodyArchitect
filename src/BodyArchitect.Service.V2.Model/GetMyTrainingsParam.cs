using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Reports;

namespace BodyArchitect.Service.V2.Model
{
    public enum MyTrainingSortOrder
    {
        Name,
        StartDate,
        PercentageCompleted
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetMyTrainingsParam : IUserParameter
    {
        public GetMyTrainingsParam()
        {
            TrainingEnds = new List<TrainingEnd>();
        }

        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }

        [DataMember]
        public DateTime? StartDate
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? EndDate
        {
            get;
            private set;
        }

        [DataMember]
        public MyTrainingSortOrder SortOrder { get; set; }

        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public IList<TrainingEnd> TrainingEnds { get; private set; }

        [DataMember]
        public Guid? MyTrainingId { get; set; }
    }
}
