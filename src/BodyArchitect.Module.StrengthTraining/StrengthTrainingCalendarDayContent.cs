using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Module.StrengthTraining.Model;
using BodyArchitect.Service.Model;


namespace BodyArchitect.Module.StrengthTraining
{
    [Export(typeof(ICalendarDayContent))]
    public class StrengthTrainingCalendarDayContent : ICalendarDayContent
    {
        public static readonly Guid ID = new Guid("0FFE7F13-87A4-4A9F-9B52-7560638A75DD");

        public Guid GlobalId
        {
            get { return ID; }
        }

        public Type SupportedEntryType
        {
            get { return typeof(StrengthTrainingEntryDTO); }
        }

        public string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects)
        {
            Dictionary<ExerciseType, int> exerciseCounter = new Dictionary<ExerciseType, int>();
            List<StrengthTrainingItemDTO> trainingDayEntries = new List<StrengthTrainingItemDTO>();
            var strengthEntries = entryObjects.Cast<StrengthTrainingEntryDTO>();

            foreach (var sizeEntry in strengthEntries)
            {
                foreach (StrengthTrainingItemDTO list in sizeEntry.Entries)
                {
                    var exercise = ObjectsReposidory.GetExercise(list.ExerciseId);
                    var exerciseType = exercise.ExerciseType;
                    if (!exerciseCounter.ContainsKey(exerciseType))
                    {
                        exerciseCounter.Add(exerciseType, 0);
                    }
                    exerciseCounter[exerciseType] = exerciseCounter[exerciseType] + list.Series.Count;
                    trainingDayEntries.Add(list);
                }
            }
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<ExerciseType, int> keyValuePair in exerciseCounter)
            {
                LocalizedExerciseType localizedType = new LocalizedExerciseType(keyValuePair.Key);
                builder.AppendLine(localizedType.LocalizedName + ":" + keyValuePair.Value);
            }
            return builder.ToString();
        }

        public Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects)
        {
            return Color.LightBlue;
        }

        public string Name
        {
            get { return StrengthTrainingEntryStrings.StrengthTrainingCalendarDayContentName; }
        }

        public Image Image
        {
            get { return StrengthTrainingResources.StrengthTrainingModule; }
        }

        public void PrepareData()
        {
            ObjectsReposidory.EnsureExercisesLoaded();
        }
    }
}
