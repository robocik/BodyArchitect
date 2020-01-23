using System;
using System.Runtime.Serialization;

namespace BodyArchitect.Service.V2.Model
{
    public enum SupplementCycleDayRepetitions
    {
        //TODO:Add validation to supplementcycle that there cannot be added days with any and trainig day. any can only be one element
        EveryDay,
        OnceAWeek,
        StrengthTrainingDay,
        CardioTrainingDay,
        NonTrainingDay
    }


    public enum DosageUnit
    {
        Absolute,
        ON10KgWight
    }


    [DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    public class SupplementsCycleDTO:MyTrainingDTO
    {
        [DataMember]
        public string TrainingDays { get; set; }

        [DataMember]
        public Guid SupplementsCycleDefinitionId { get; set; }

        [DataMember]
        public decimal Weight { get; set; }

        [DataMember]
        public int TotalWeeks { get; set; }
    }
}
