using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    [Serializable]
    public class ExerciseProfileData : FMGlobalObject
    {
        public virtual Exercise Exercise { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual decimal MaxWeight { get; set; }

        public virtual decimal? Repetitions { get; set; }

        public virtual DateTime TrainingDate { get; set; }

        public virtual Serie Serie { get; set; }
    }
}
