using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public class WorkoutPlanEventArgs:EventArgs
    {
        public WorkoutPlanEventArgs(WorkoutPlanDTO workoutPlan)
        {
            WorkoutPlan = workoutPlan;
        }

        public WorkoutPlanDTO WorkoutPlan { get; private set; }
    }
}
