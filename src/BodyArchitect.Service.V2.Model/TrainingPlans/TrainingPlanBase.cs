using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    public enum TrainingType
    {
        Split,
        FBW,
        HIT,
        ABW,
        HST,
        PushPull,
        ACT,
        Other
    }

    //public enum TrainingPlanSerieRepetitions
    //{
    //    Normalna,
    //    Max,
    //    MuscleFailure,
    //    Rozgrzewkowa,
    //    PrawieMax
        
    //}

    //[Serializable]
    //[DataContract(Namespace = Const.ServiceNamespace, IsReference = true)]
    //public abstract class TrainingPlanBase
    //{
    //    private Guid globalId = Guid.NewGuid();

    //    [DataMember]
    //    [SerializerId]
    //    [DoNotChecksum]
    //    public Guid GlobalId
    //    {
    //        get { return globalId; }
    //        set { globalId = value; }
    //    }

    //    [OnDeserialized()]
    //    internal void OnDeserializedMethod(StreamingContext context)
    //    {
    //        if (context.State == StreamingContextStates.Clone)
    //        {
    //            GlobalId = Guid.NewGuid();
    //        }
    //    }

    //}


    public enum TrainingPlanDifficult
    {
        NotSet,
        Beginner,
        Advanced,
        Professional
    }
}
