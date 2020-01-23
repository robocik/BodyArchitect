using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class ExerciseRecordViewModel : ViewModelBase
    {
        private ExerciseRecordsReportResultItem item;

        public ExerciseRecordViewModel(ExerciseRecordsReportResultItem item)
        {
            this.item = item;
        }

        public bool CalendarAvailable
        {
            get { return item.User.IsMe || item.User.HaveAccess(item.User.Privacy.CalendarView); }
        }

        public ExerciseRecordsReportResultItem Item
        {
            get { return item; }
        }

        public string CardioValue
        {
            get
            {
                var time = TimeSpan.FromSeconds((double)item.MaxWeight);
                return time.ToString();
            }
        }

        public bool IsCardio
        {
            get { return item.Exercise.ExerciseType == ExerciseType.Cardio; }
        }

        public decimal Weight
        {
            get { return item.MaxWeight.ToDisplayWeight(); }
        }

        public string WeightType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.WeightType == Service.V2.Model.WeightType.Kg)
                {
                    return ApplicationStrings.Kg;
                }
                else
                {
                    return ApplicationStrings.Pound;
                }
            }
        }
    }
}
