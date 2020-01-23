using System;
using System.Collections.Generic;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public class ExercisesFilteredComboBox : CachedFilteredComboBox<ExerciseDTO>
    {
        protected override Common.ObjectsCacheBase<ExerciseDTO> GetCache()
        {
            return ExercisesReposidory.Instance;
        }

        public ExerciseType? ShowOnlyExerciseType { get; set; }

        protected override bool FilterCollection(ExerciseDTO item)
        {
            if (ShowOnlyExerciseType.HasValue)
            {
                return item.ExerciseType == ShowOnlyExerciseType.Value;
            }
            return base.FilterCollection(item);
        }
        protected override bool FilterImplementation(object value)
        {
            var exercise = (ExerciseLightDTO)value;
            string text = this.Text.ToLower();
            return exercise.GetLocalizedName().ToLower().Contains(text)
                || exercise.Shortcut.ToLower().Contains(text)
                || EnumLocalizer.Default.Translate(exercise.ExerciseType).ToLower().Contains(text);
        }
    }
}
