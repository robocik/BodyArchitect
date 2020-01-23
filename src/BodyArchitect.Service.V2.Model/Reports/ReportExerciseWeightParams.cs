using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model.Reports
{
    public interface IUserParameter
    {
        Guid? UserId { get; set; }

        Guid? CustomerId { get; set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ReportExerciseWeightParams : IUserParameter
    {
        public ReportExerciseWeightParams()
        {
            Exercises = new List<Guid>();
            MyPlaces= new List<Guid>();
            SetTypes = new List<SetType>();
            DoneWays=new List<ExerciseDoneWay>();
        }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<Guid> Exercises { get; private set; }

        [NotNullValidator]
        [DataMember]
        public List<ExerciseDoneWay> DoneWays { get; private set; }

        [NotNullValidator]
        [DataMember]
        public List<Guid> MyPlaces { get; private set; }

        [DataMember]
        public bool UseAllEntries { get; set; }

        [DataMember]
        public bool? RestPause { get; set; }

        [DataMember]
        public bool? SuperSlow { get; set; }

        [DataMember]
        public int? RepetitionsFrom { get; set; }

        [DataMember]
        public int? RepetitionsTo { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<SetType> SetTypes { get; private set; }

        [DataMember]
        public Guid? UserId { get; set; }

        [DataMember]
        public Guid? CustomerId { get; set; }
    }
}
