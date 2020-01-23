using System;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Module.StrengthTraining.Model
{
    public class LocalizedExerciseType
    {
        private ExerciseType exerciseType;

        public LocalizedExerciseType(ExerciseType ExerciseType)
        {
            this.exerciseType = ExerciseType;
        }

        public string Name
        {
            get
            {
                return Enum.GetName(typeof(ExerciseType), ExerciseType);
            }
        }

        public string LocalizedName
        {
            get
            {
                return EnumLocalizer.Default.Translate(ExerciseType);
            }
        }

        public ExerciseType ExerciseType
        {
            get { return exerciseType; }
        }

        public override string ToString()
        {
            return LocalizedName;
        }
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                LocalizedExerciseType type1 = (LocalizedExerciseType) obj;
                return type1.exerciseType == ExerciseType;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ExerciseType.GetHashCode();
        }
    }
}
