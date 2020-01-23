using System;

using System.Runtime.Serialization;


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

    public enum TrainingPlanSerieRepetitions
    {
        Normalna,
        Max,
        MuscleFailure,
        Rozgrzewkowa,
        PrawieMax
        
    }

    public class TrainingPlanBase
    {
        private Guid globalId = Guid.NewGuid();

        public Guid GlobalId
        {
            get { return globalId; }
            set { globalId = value; }
        }
    }


    public enum TrainingPlanDifficult
    {
        NotSet,
        Beginner,
        Advanced,
        Professional
    }
}
