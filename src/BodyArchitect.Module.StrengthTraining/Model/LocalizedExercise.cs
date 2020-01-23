using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Model
{
    public class LocalizedExercise
    {
        private ExerciseDTO exercise;

        public LocalizedExercise(ExerciseDTO exercise)
        {
            this.exercise = exercise;
        }

        public Guid GlobalId
        {
            get
            {
                return ExerciseDTO.GlobalId;
            }
        }

        public string Shortcut
        {
            get
            {
                return ExerciseDTO.GetLocalizedShortcut();
            }
        }

        public string Name
        {
            get { return ExerciseDTO.GetLocalizedName(); }
        }

        public string Muscle
        {
            get
            {
                return EnumLocalizer.Default.Translate(ExerciseDTO.ExerciseType);
            }
        }

        public ExerciseType ExerciseTypeValue
        {
            get
            {
                return ExerciseDTO.ExerciseType;
            }
        }

        public ExerciseDTO ExerciseDTO
        {
            get { return exercise; }
        }
    }
}
