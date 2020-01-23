using System;
using System.Linq;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.Module.StrengthTraining.Model
{
    static class ModelExtensions
    {
        public static bool IsCardio(this SerieDTO serie)
        {
            return IsCardio(serie.StrengthTrainingItem);
        }

        public static bool IsCardio(this StrengthTrainingItemDTO item)
        {
            var exercise = item.Exercise;
            return exercise!=null?exercise.ExerciseType == ExerciseType.Cardio:false;
        }

        

        public static bool IsEditable(this ExerciseDTO exercise)
        {
            if (exercise == null)
            {
                return false;
            }
            if (exercise.Profile == null || !exercise.IsNew)
            {
                return false;
            }
            return true;
        }

        public static bool IsFavorite(this ExerciseLightDTO exercise)
        {
            ExercisesReposidory.Instance.EnsureLoaded();
            var res = (from e in ExercisesReposidory.Instance.Items.Values where e.GlobalId == exercise.GlobalId && e.Profile != null && !((IBelongToUser)e).IsMine() select e).Count();
            return res > 0;
        }



        public static bool IsFavorite(this TrainingPlan plan)
        {
            WorkoutPlansReposidory.Instance.EnsureLoaded();
            var res = (from e in WorkoutPlansReposidory.Instance.Items.Values where e.GlobalId == plan.GlobalId && e.Profile != null && !e.IsMine() select e).Count();
            return res > 0;
        }
    }
}
