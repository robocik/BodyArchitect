using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace BodyArchitect.Service.V2.Model.TrainingPlans
{

    public class TrainingPlanEntry : TrainingPlanBase
    {
        public TrainingPlanEntry()
        {
            Sets = new List<TrainingPlanSerie>();
        }

        private Guid exerciseId;

        
        public List<TrainingPlanSerie> Sets { get; set; }

        
        public int RestSeconds { get; set; }

        public TrainingPlanDay Day { get; set; }

        
        public Guid ExerciseId
        {
            get { return exerciseId; }
            set { exerciseId = value; }
        }

        public int Position
        {
            get
            {
                if (Day != null)
                {
                    return Day.Entries.IndexOf(this);
                }
                return -1;
            }
            set { }
        }


        [DataMember]
        public string Comment
        {
            get; set;
        }

    }
}
