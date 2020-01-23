using System;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
namespace BodyArchitect.WP7.Controls.Model
{
    public static class TrainingPlanHelper
    {
        public static string GetDisplayText(this TrainingPlanSerie serie)
        {
            string dropSetString = "";
            string repType = "";
            string superSlow = "";
            string restPause = "";

            if (serie.IsSuperSlow)
            {
                superSlow = " SS";
            }
            if (serie.IsRestPause)
            {
                restPause = " RP";
            }
            if (serie.DropSet != DropSetType.None)
            {
                dropSetString = "^" + (int)serie.DropSet;
            }
            if (serie.RepetitionsType != SetType.Normalna)
            {
                repType = EnumLocalizer.Default.Translate(serie.RepetitionsType);
            }
            string format = serie.ToStringRepetitionsRange();

            return string.Format("{0} {1}{2}{3}{4}", format, repType, dropSetString,superSlow,restPause);
        }

        private static string getShortSetType(SetType setType)
        {
            if (setType == SetType.Max)
            {
                return ApplicationStrings.SetType_Long_Max;
            }
            if (setType == SetType.MuscleFailure)
            {
                return ApplicationStrings.SetType_Long_MuscleFailure;
            }
            if (setType == SetType.PrawieMax)
            {
                return ApplicationStrings.SetType_Long_PrawieMax;
            }
            if (setType == SetType.Rozgrzewkowa)
            {
                return ApplicationStrings.SetType_Long_Rozgrzewkowa;
            }
            return "";
        }

        public static string GetDisplayText(this SerieDTO serie, bool longSetType=true)
        {
            string dropSetString = "";
            string repType = "";
            string superSlow = "";
            if (serie.StrengthTrainingItem.Exercise.ExerciseType==ExerciseType.Cardio)
            {
                decimal seconds = serie.Weight.HasValue ? serie.Weight.Value : 0;
                var time = TimeSpan.FromSeconds((double)Math.Round(seconds,0));
                return time.ToString();
            }
            if (serie.DropSet != DropSetType.None)
            {
                dropSetString = "^" + (int)serie.DropSet;
            }
            if(serie.IsSuperSlow)
            {
                superSlow = "SS";
            }
            if (serie.SetType != SetType.Normalna)
            {
                if (longSetType)
                {
                    repType = EnumLocalizer.Default.Translate(serie.SetType);
                }
                else
                {
                    repType = getShortSetType(serie.SetType);

                }
            }
            decimal ? weight = null;
            if(serie.Weight!=null)
            {
                weight = (decimal)serie.Weight.Value.ToDisplayWeight();
            }
            string format = string.Format("{0:#}x{1:#.##}", serie.RepetitionNumber, weight);

            return string.Format("{0} {1}{2}{3}", format, repType, dropSetString,superSlow);
        }
    }
}
