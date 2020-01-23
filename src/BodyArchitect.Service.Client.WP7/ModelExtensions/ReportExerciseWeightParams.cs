using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public partial class ReportExerciseWeightParams
    {
        public ReportExerciseWeightParams()
        {
            MyPlaces = new List<Guid>();
            Exercises = new List<Guid>();
            this.DoneWays = new List<ExerciseDoneWay>();
            this.SetTypes = new List<SetType>();
        }
    }
}
