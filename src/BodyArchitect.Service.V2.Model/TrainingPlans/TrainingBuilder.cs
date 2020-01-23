using System;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    public partial class TrainingBuilder
    {
        public bool FillRepetitionNumber { get; set; }

        public void FillTrainingFromPlan(TrainingPlanDay planDay, StrengthTrainingEntryDTO strengthEntry)
        {
            strengthEntry.Entries.Clear();
            foreach (TrainingPlanEntry trainingPlanEntry in planDay.Entries)
            {
                var strengthItem = new StrengthTrainingItemDTO();
                strengthItem.Exercise = trainingPlanEntry.Exercise;
                strengthItem.DoneWay = trainingPlanEntry.DoneWay;
                strengthItem.SuperSetGroup = trainingPlanEntry.GroupName;
                strengthItem.TrainingPlanItemId = trainingPlanEntry.GlobalId;
                strengthEntry.AddEntry(strengthItem);
                foreach (var set in trainingPlanEntry.Sets)
                {
                    var serie = new SerieDTO();
                    if(strengthItem.Exercise.ExerciseType==ExerciseType.Cardio)
                    {
                        if (FillRepetitionNumber &&  set.RepetitionNumberMin.HasValue)
                        {
                            serie.Weight = set.RepetitionNumberMin.Value;
                        }
                    }
                    else
                    {
                        if (FillRepetitionNumber && set.RepetitionNumberMax.HasValue && set.RepetitionNumberMin.HasValue && set.RepetitionNumberMax == set.RepetitionNumberMin)
                        {
                            serie.RepetitionNumber = set.RepetitionNumberMax.Value;
                        }
                    }


                    serie.IsSuperSlow = set.IsSuperSlow;
                    serie.IsRestPause = set.IsRestPause;
                    serie.DropSet = set.DropSet;
                    serie.SetType = (SetType) set.RepetitionsType;
                    serie.TrainingPlanItemId = set.GlobalId;
                    strengthItem.AddSerie(serie);
                }
            }
            strengthEntry.TrainingPlanItemId = planDay.GlobalId;
            strengthEntry.TrainingPlanId = planDay.TrainingPlan.GlobalId;
        }

    }
}
