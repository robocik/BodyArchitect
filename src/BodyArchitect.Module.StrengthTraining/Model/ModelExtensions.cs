using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Model
{
    static class ModelExtensions
    {
        public static bool IsCardio(this SerieDTO serie)
        {
            var exercise =ObjectsReposidory.GetExercise(serie.StrengthTrainingItem.ExerciseId);
            return exercise.ExerciseType == ExerciseType.Cardio;
        }
    }
}
