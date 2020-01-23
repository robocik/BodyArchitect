using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Module.StrengthTraining.Model;
using BodyArchitect.Service.V2.Model;


namespace BodyArchitect.Client.Module.StrengthTraining
{
    //[Export(typeof(ICalendarDayContent))]
    //public class StrengthTrainingCalendarDayContent : ICalendarDayContent
    //{
    //    //public static readonly Guid ID = new Guid("0FFE7F13-87A4-4A9F-9B52-7560638A75DD");
    //    public static readonly Guid ID = new Guid("F69099BE-C42A-4EC5-B582-5EFF57758503");

    //    public Guid GlobalId
    //    {
    //        get { return ID; }
    //    }

    //    public Type SupportedEntryType
    //    {
    //        get { return typeof(StrengthTrainingEntryDTO); }
    //    }

    //    public string GetDayInfoText(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        Dictionary<ExerciseType, int> exerciseCounter = new Dictionary<ExerciseType, int>();
    //        List<StrengthTrainingItemDTO> trainingDayEntries = new List<StrengthTrainingItemDTO>();
    //        var strengthEntries = entryObjects.Cast<StrengthTrainingEntryDTO>();

    //        foreach (var sizeEntry in strengthEntries)
    //        {
    //            foreach (StrengthTrainingItemDTO list in sizeEntry.Entries)
    //            {
    //                var exercise = list.Exercise;
    //                var exerciseType = exercise.ExerciseType;
    //                if (!exerciseCounter.ContainsKey(exerciseType))
    //                {
    //                    exerciseCounter.Add(exerciseType, 0);
    //                }
    //                exerciseCounter[exerciseType] = exerciseCounter[exerciseType] + list.Series.Count;
    //                trainingDayEntries.Add(list);
    //            }
    //        }
    //        StringBuilder builder = new StringBuilder();
    //        foreach (KeyValuePair<ExerciseType, int> keyValuePair in exerciseCounter)
    //        {
    //            LocalizedExerciseType localizedType = new LocalizedExerciseType(keyValuePair.Key);
    //            builder.AppendLine(localizedType.LocalizedName + ": " + keyValuePair.Value);
    //        }
    //        return builder.ToString();
    //    }

    //    public Color GetBackColor(IEnumerable<EntryObjectDTO> entryObjects)
    //    {
    //        return Colors.LightBlue;
    //    }

    //    public string Name
    //    {
    //        get { return StrengthTrainingEntryStrings.StrengthTrainingCalendarDayContentName; }
    //    }

    //    public ImageSource Image
    //    {
    //        get 
    //        {
    //            return "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/StrengthTraining.png".ToBitmap();
    //        }
    //    }
    //}


    [Export(typeof(ICalendarDayContextEx))]
    public class StrengthTrainingCalendarDayContentEx : ICalendarDayContextEx
    {
        //public static readonly Guid ID = new Guid("0FFE7F13-87A4-4A9F-9B52-7560638A75DD");
        public static readonly Guid ID = new Guid("F69099BE-C42A-4EC5-B582-5EFF57758503");

        public string Name
        {
            get { return StrengthTrainingEntryStrings.StrengthTrainingCalendarDayContentName; }
        }

        public ImageSource Image
        {
            get
            {
                return "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/StrengthTraining.png".ToBitmap();
            }
        }

        public Guid GlobalId
        {
            get { return ID; }
        }

        public ImageItem[] GetDayContents(TrainingDayDTO day)
        {
            List<ImageItem> items = new List<ImageItem>();
            Dictionary<ExerciseType, int> exerciseCounter = new Dictionary<ExerciseType, int>();
            List<StrengthTrainingItemDTO> trainingDayEntries = new List<StrengthTrainingItemDTO>();
            foreach(var entry in day.Objects.OfType<StrengthTrainingEntryDTO>())
            {
                foreach (StrengthTrainingItemDTO list in entry.Entries)
                {
                    var exercise = list.Exercise;
                    var exerciseType = exercise.ExerciseType;
                    if (!exerciseCounter.ContainsKey(exerciseType))
                    {
                        exerciseCounter.Add(exerciseType, 0);
                    }

                    exerciseCounter[exerciseType] = exerciseCounter[exerciseType] + list.Series.Count;
                    trainingDayEntries.Add(list);
                }


                StringBuilder builder = new StringBuilder();
                foreach (KeyValuePair<ExerciseType, int> keyValuePair in exerciseCounter)
                {
                    LocalizedExerciseType localizedType = new LocalizedExerciseType(keyValuePair.Key);
                    builder.AppendLine(localizedType.LocalizedName + ": " + keyValuePair.Value);
                }

                ImageItem item = new ImageItem();
                item.BackBrush = EntryObjectColors.StrengthTraining;
                item.Content = builder.ToString();
                item.Entry = entry;
                item.ToolTip = Name;
                item.Image = Image;
                items.Add(item);

            }

            return items.ToArray();
        }
    }
}
