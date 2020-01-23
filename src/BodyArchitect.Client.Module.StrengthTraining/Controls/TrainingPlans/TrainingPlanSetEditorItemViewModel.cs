using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls.TrainingPlans
{
    public class TrainingPlanSetEditorItemViewModel : DynamicObject
    {
        private TrainingPlanEntry planEntry;
        private ExerciseLightDTO exercise;

        public TrainingPlanSetEditorItemViewModel(TrainingPlanEntry planEntry)
        {
            this.planEntry = planEntry;
            exercise = planEntry.Exercise;
        }

        public string ExerciseName
        {
            get
            {
                return Entry.Exercise.GetLocalizedName();
            }
        }

        public SolidColorBrush Background { get; set; }

        public string Day
        {
            get { return Entry.Day.Name; }
        }

        public string ExerciseType
        {
            get { return EnumLocalizer.Default.Translate(exercise.ExerciseType); }
        }

        public TrainingPlanEntry Entry
        {
            get { return planEntry; }
        }

        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name;
            int setNumber = int.Parse(name);
            result = string.Empty;
            if(Entry.Sets.Count>setNumber)
            {
                result = Entry.Sets[setNumber].ToStringRepetitionsRange();
            }
            return true;

        }

        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            
            return true;
        }
    }
}
