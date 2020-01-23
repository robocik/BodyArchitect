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
    public class CardioSessionViewModel : SetViewModel
    {
        public CardioSessionViewModel(SerieDTO set):base(set)
        {

        }

        public override string SetIndexTitle
        {
            get { return string.Format(ApplicationStrings.SetViewModel_CardioSessionIndexTitle, Set.StrengthTrainingItem.Series.IndexOf(Set) + 1); }
        }

        public decimal? Distance
        {
            get
            {
                if (Set.RepetitionNumber.HasValue)
                {
                    return Set.RepetitionNumber.Value.ToDisplayDistance();
                }
                return null;
            }
            set
            {
                Set.RepetitionNumber = value.HasValue?value.Value.FromDisplayDistance():(decimal?) null;
                NotifyPropertyChanged("Distance");
            }
        }

        public bool HasDistance
        {
            get { return Distance.HasValue; }
        }

        public bool HasCalories
        {
            get { return Set.CalculatedValue.HasValue; }
        }

        protected override void TimerCalculation()
        {
            Set.Weight = (decimal)(Set.EndTime.Value - Set.StartTime.Value).TotalSeconds;
            if (ApplicationState.Current.TrainingDay.TrainingDay.CustomerId.HasValue)
            {
            
                var customer = ApplicationState.Current.Cache.Customers.GetItem(ApplicationState.Current.TrainingDay.TrainingDay.CustomerId.Value);
                Calories=GPSTrackerViewModel.CalculateCalories(this.Set.StrengthTrainingItem.Exercise.Met,Set.Weight,customer);
            }
            else
            {
                Calories = GPSTrackerViewModel.CalculateCalories(this.Set.StrengthTrainingItem.Exercise.Met, Set.Weight, ApplicationState.Current.ProfileInfo);
            }
        }

        public decimal? Calories
        {
            get
            {
                if (Set.CalculatedValue.HasValue)
                {
                    return Set.CalculatedValue.Value;
                }
                return 0;
            }
            set
            {
                Set.CalculatedValue = value;
                NotifyPropertyChanged("Calories");
            }
        }


        public override string ToString()
        {
            return CardioSessionTime.ToString();
        }
    }
}
