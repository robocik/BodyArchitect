using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.Module.StrengthTraining.Model;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls.PlansUI;
using BodyArchitect.Client.UI.Converters;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public class WorkoutPlanViewModel : PlanViewModel
    {
        public WorkoutPlanViewModel(TrainingPlan plan, FeaturedItem featured = FeaturedItem.None)
            :base(plan,featured)
        {

        }

        public string Type
        {
            get { return EnumLocalizer.Default.Translate(Plan.TrainingType); }
        }

        public string Difficult
        {
            get { return EnumLocalizer.Default.Translate(Plan.Difficult); }
        }

        public string Purpose
        {
            get { return EnumLocalizer.Default.Translate(Plan.Purpose); }
        }

        public int Days
        {
            get { return Plan.Days.Count; }
        }

        

        public new TrainingPlan Plan
        {
            get { return (TrainingPlan) base.Plan; }
        }


        protected override bool IsFavorite()
        {
            return Plan.IsFavorite();
        }
    }
}
