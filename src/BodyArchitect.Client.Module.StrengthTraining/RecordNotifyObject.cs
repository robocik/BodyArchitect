using System;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining
{
    public class RecordNotifyObject : NotifyObjectBase
    {
        public RecordNotifyObject(SerieDTO set)
            : base(DateTime.UtcNow)
        {
            ExerciseName = set.StrengthTrainingItem.Exercise.Name;
            IsCardio = set.StrengthTrainingItem.Exercise.ExerciseType == ExerciseType.Cardio;
            if (IsCardio)
            {
                var time = TimeSpan.FromSeconds((double)set.Weight);
                CardioValue= time.ToString();
            }
            else
            {
                Repetitions = (int)set.RepetitionNumber.Value;
                Weight = set.Weight.Value.ToDisplayWeight();    
            }
            

        }

        public bool IsCardio { get; set; }

        public string ExerciseName { get; set; }

        public int Repetitions { get; set; }

        public decimal Weight { get; set; }

        public string CardioValue { get; set; }

        public string WeightType
        {
            get
            {
                if (UserContext.Current.ProfileInformation.Settings.WeightType == Service.V2.Model.WeightType.Pounds)
                {
                    return Strings.WeightType_Pound;
                }
                else
                {
                    return Strings.WeightType_Kg;
                }
            }
        }
    }
}
